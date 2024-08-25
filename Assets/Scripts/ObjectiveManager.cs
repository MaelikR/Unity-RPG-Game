using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections;
/*
 * -----------------------------------------------------------------------------
 *  Project:        RPG Game
 *  Script:         ObjectiveManager.cs
 *  Description:    Manages the objectives in the game, including tracking enemy
 *                  kills and boss defeats. Handles data persistence using 
 *                  PlayFab and UI updates via Unity's UI system. Syncs objective 
 *                  data across the network using Mirror.
 * 
 *  Author:         M.Ren
 *  Date:           [25/08/2024]
 *  Version:        1.0 (Debugging in Progress)
 * 
 *  Unity Version:  [2021.3.8]
 *  Mirror Version: [Mirror 2022.9.15]
 *  PlayFab SDK:    [PlayFab SDK 2.152.221010]
 * 
 *  Usage:          Attach this script to an empty GameObject in the scene.
 *                  Ensure that the necessary UI Text components are assigned 
 *                  in the inspector, and that the PlayFab API is configured 
 *                  correctly in the Unity project.
 * 
 *  Notes:          - This script relies on Mirror for networking and PlayFab
 *                    for data management.
 *                  - UI updates occur every second to minimize network traffic.
 *                  - The SerializableDictionaryComplex class is used for 
 *                    serializing enemy kill data.
 * 
 *  -----------------------------------------------------------------------------
 *  License:        This script is provided as-is without any guarantees.
 *                  Modify it freely for your project needs.
 * -----------------------------------------------------------------------------
 */

public class ObjectiveManager : NetworkBehaviour
{
    public Text enemiesKilledText;
    public Text objectiveText;

    private Dictionary<string, int> enemiesKilledCount = new Dictionary<string, int>();
    private int totalEnemiesKilled;
    private bool bossDefeated;
    private Coroutine uiUpdateCoroutine;

    void Start()
    {
        if (isLocalPlayer)
        {
            SetObjectiveUIInteractive(true);
            enemiesKilledCount = new Dictionary<string, int>();
            totalEnemiesKilled = 0;
            bossDefeated = false;
            LoadObjectiveData();
            uiUpdateCoroutine = StartCoroutine(UpdateUIRoutine());
        }
        else
        {
            SetObjectiveUIInteractive(false);
        }
    }

    void SetObjectiveUIInteractive(bool isInteractive)
    {
        if (enemiesKilledText != null && objectiveText != null)
        {
            enemiesKilledText.gameObject.SetActive(isInteractive);
            objectiveText.gameObject.SetActive(isInteractive);
        }
    }

    [Server]
    public void RegisterEnemyKill(string enemyType)
    {
        if (string.IsNullOrEmpty(enemyType))
        {
            UnityEngine.Debug.LogWarning("Attempted to register kill with a null or empty enemyType.");
            return;
        }

        if (!enemiesKilledCount.ContainsKey(enemyType))
        {
            enemiesKilledCount[enemyType] = 0;
        }

        enemiesKilledCount[enemyType]++;
        totalEnemiesKilled++;
        RpcUpdateUI(new List<string>(enemiesKilledCount.Keys), new List<int>(enemiesKilledCount.Values), bossDefeated);
        SaveObjectiveData();
    }

    [Server]
    public void RegisterBossDefeat()
    {
        if (bossDefeated)
        {
            UnityEngine.Debug.LogWarning("Boss has already been defeated.");
            return;
        }

        bossDefeated = true;
        RpcUpdateUI(new List<string>(enemiesKilledCount.Keys), new List<int>(enemiesKilledCount.Values), bossDefeated);
        SaveObjectiveData();
    }

    [ClientRpc]
    void RpcUpdateUI(List<string> enemyTypes, List<int> killCounts, bool updatedBossDefeated)
    {
        enemiesKilledCount.Clear();
        for (int i = 0; i < enemyTypes.Count; i++)
        {
            enemiesKilledCount[enemyTypes[i]] = killCounts[i];
        }
        bossDefeated = updatedBossDefeated;
        UpdateUI();
    }

    private IEnumerator UpdateUIRoutine()
    {
        while (true)
        {
            UpdateUI();
            yield return new WaitForSeconds(1f); // Update the UI every second
        }
    }

    void UpdateUI()
    {
        if (!isLocalPlayer) return;

        enemiesKilledText.text = "Enemies Killed:\n";
        foreach (var entry in enemiesKilledCount)
        {
            enemiesKilledText.text += $"{entry.Key}: {entry.Value}\n";
        }

        if (bossDefeated)
        {
            objectiveText.text = "Objective: Boss Defeated!";
        }
        else
        {
            objectiveText.text = "Objective: Defeat the Boss!";
        }
    }

    void SaveObjectiveData()
    {
        var serializableEnemiesKilledCount = new SerializableDictionaryComplex(enemiesKilledCount);

        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                { "EnemiesKilled", JsonUtility.ToJson(serializableEnemiesKilledCount) },
                { "BossDefeated", bossDefeated.ToString() }
            }
        };

        PlayFabClientAPI.UpdateUserData(request, result =>
        {
            UnityEngine.Debug.Log("Objective data saved successfully");
        }, error =>
        {
            UnityEngine.Debug.LogError("Error saving objective data: " + error.GenerateErrorReport());
        });
    }

    void LoadObjectiveData()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), result =>
        {
            if (result.Data == null || result.Data.Count == 0)
            {
                UnityEngine.Debug.Log("No objective data found, initializing with defaults.");
                enemiesKilledCount = new Dictionary<string, int>();
                bossDefeated = false;
                UpdateUI();
                return;
            }

            if (result.Data.ContainsKey("EnemiesKilled"))
            {
                var serializableEnemiesKilledCount = JsonUtility.FromJson<SerializableDictionaryComplex>(result.Data["EnemiesKilled"].Value);
                enemiesKilledCount = serializableEnemiesKilledCount.ToDictionary();
            }
            if (result.Data.ContainsKey("BossDefeated"))
            {
                bossDefeated = bool.Parse(result.Data["BossDefeated"].Value);
            }
            UpdateUI();
        }, error =>
        {
            UnityEngine.Debug.LogError("Error loading objective data: " + error.GenerateErrorReport());
        });
    }

    void OnDestroy()
    {
        if (uiUpdateCoroutine != null)
        {
            StopCoroutine(uiUpdateCoroutine);
        }
    }
}

[System.Serializable]
public class CustomEnemyData
{
    public string enemyType;
    public int kills;

    public CustomEnemyData(string type, int kills)
    {
        this.enemyType = type;
        this.kills = kills;
    }
}

[System.Serializable]
public class SerializableDictionaryComplex
{
    public List<CustomEnemyData> enemyDataList = new List<CustomEnemyData>();

    public SerializableDictionaryComplex(Dictionary<string, int> dict)
    {
        foreach (var kvp in dict)
        {
            enemyDataList.Add(new CustomEnemyData(kvp.Key, kvp.Value));
        }
    }

    public Dictionary<string, int> ToDictionary()
    {
        var dict = new Dictionary<string, int>();
        foreach (var data in enemyDataList)
        {
            dict[data.enemyType] = data.kills;
        }
        return dict;
    }
}
