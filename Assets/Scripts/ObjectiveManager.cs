using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using PlayFab;
using PlayFab.ClientModels;


public class ObjectiveManager : NetworkBehaviour
{
    public Text enemiesKilledText;
    public Text objectiveText;

    private Dictionary<string, int> enemiesKilledCount = new Dictionary<string, int>();
    private int totalEnemiesKilled;
    private bool bossDefeated;

    void Start()
    {
        if (isLocalPlayer)
        {
            SetObjectiveUIInteractive(true); // Activer les éléments interactifs du UI des objectifs pour le joueur local
            enemiesKilledCount = new Dictionary<string, int>();
            totalEnemiesKilled = 0;
            bossDefeated = false;
            LoadObjectiveData();
        }
        else
        {
            SetObjectiveUIInteractive(false); // Désactiver les éléments interactifs du UI des objectifs pour les joueurs distants
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
        var serializableEnemiesKilledCount = new SerializableDictionary<string, int>(enemiesKilledCount);

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
            if (result.Data.ContainsKey("EnemiesKilled"))
            {
                var serializableEnemiesKilledCount = JsonUtility.FromJson<SerializableDictionary<string, int>>(result.Data["EnemiesKilled"].Value);
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
}

[System.Serializable]
public class SerializableDictionary<TKey, TValue>
{
    public List<TKey> Keys = new List<TKey>();
    public List<TValue> Values = new List<TValue>();

    public SerializableDictionary() { }

    public SerializableDictionary(Dictionary<TKey, TValue> dict)
    {
        foreach (var kvp in dict)
        {
            Keys.Add(kvp.Key);
            Values.Add(kvp.Value);
        }
    }

    public Dictionary<TKey, TValue> ToDictionary()
    {
        var dict = new Dictionary<TKey, TValue>();
        for (int i = 0; i < Keys.Count; i++)
        {
            dict[Keys[i]] = Values[i];
        }
        return dict;
    }
}
