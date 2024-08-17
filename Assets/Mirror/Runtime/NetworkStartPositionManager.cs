using UnityEngine;
using System.Collections.Generic;

namespace Mirror
{
    public class NetworkStartPositionManager : MonoBehaviour
    {
        public enum SpawnType { Default, Faction, LevelBased, QuestBased }

        private Dictionary<SpawnType, List<Transform>> spawnPoints = new Dictionary<SpawnType, List<Transform>>();

        public void RegisterStartPosition(Transform startPosition, SpawnType spawnType = SpawnType.Default)
        {
            if (!spawnPoints.ContainsKey(spawnType))
            {
                spawnPoints[spawnType] = new List<Transform>();
            }
            spawnPoints[spawnType].Add(startPosition);
        }

        public void UnRegisterStartPosition(Transform startPosition, SpawnType spawnType = SpawnType.Default)
        {
            if (spawnPoints.ContainsKey(spawnType))
            {
                spawnPoints[spawnType].Remove(startPosition);
            }
        }

        public Transform GetStartPosition(SpawnType spawnType = SpawnType.Default)
        {
            if (spawnPoints.ContainsKey(spawnType) && spawnPoints[spawnType].Count > 0)
            {
                int index = UnityEngine.Random.Range(0, spawnPoints[spawnType].Count);
                return spawnPoints[spawnType][index];
            }
            if (spawnPoints.ContainsKey(SpawnType.Default) && spawnPoints[SpawnType.Default].Count > 0)
            {
                int index = UnityEngine.Random.Range(0, spawnPoints[SpawnType.Default].Count);
                return spawnPoints[SpawnType.Default][index];
            }
            return null;
        }

        public Transform GetStartPositionBasedOnPlayerData(PlayerData playerData)
        {
            if (playerData.Faction != null)
            {
                Transform factionSpawn = GetStartPosition(SpawnType.Faction);
                if (factionSpawn != null) return factionSpawn;
            }
            if (playerData.Level > 10)
            {
                Transform levelSpawn = GetStartPosition(SpawnType.LevelBased);
                if (levelSpawn != null) return levelSpawn;
            }
            return GetStartPosition(SpawnType.Default);
        }
    }
}
