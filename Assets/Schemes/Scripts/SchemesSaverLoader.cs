using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cysharp.Threading.Tasks;
using GameLogic;
using Misc;
using Newtonsoft.Json;
using Schemes.Data;
using UnityEngine;

namespace Schemes
{
    [Serializable]
    public struct PlayerData
    {
        public List<Scheme> schemes;
    }
    
    public static class SchemesSaverLoader
    {
        private static readonly string SavePath = Path.Combine(Application.persistentDataPath, "PlayerData.json");
        public  static PlayerData PlayerData;

        private static List<Scheme> GetDefaultSchemes()
        {
            var schemeDatas = GameManager.Instance.GetContainerOfType<ConfigsContainer>()
                .DefaultScriptableSchemesSo
                .DefaultSchemesDataList;

            return schemeDatas.Select(schemeData => new Scheme(schemeData)).ToList();
        }
        
        public static async UniTask<List<Scheme>> LoadSchemes()
        {
            var defaultSchemes = GetDefaultSchemes();
            var playerSchemes = await GetPlayerSchemes();
            
            if (playerSchemes == null) return defaultSchemes;
            return defaultSchemes.Union(playerSchemes).ToList();
        }
        
        private static async UniTask<List<Scheme>> GetPlayerSchemes()
        {
            PlayerData = await LoadPlayerData();
            return PlayerData.schemes;
        }
        
        // Note: will not be used probably
        public static async UniTask<Scheme> LoadSchemeByName()
        {
            // todo: deserialization from json file here 
            throw new NotImplementedException("Load scheme by name is not implemented");
        }

        public static async UniTask<Scheme> LoadSchemeByKey(SchemeKey guidStr)
        {
            // todo: deserialization from json file here 
            throw new NotImplementedException("Load scheme by key is not implemented");
        }

        public static async UniTask SaveScheme(Scheme scheme)
        {
            var previousSameSchemeIndex = PlayerData.schemes.IndexOf(x => x == scheme);
            if (previousSameSchemeIndex != -1)
            {
                PlayerData.schemes[previousSameSchemeIndex] = scheme;
            }
            else
            {
                PlayerData.schemes.Add(scheme);
            }

            await SavePlayerData(PlayerData);
        }

        // public static async UniTask<bool> SaveSchemes(List<Scheme> schemes)
        // {
        //     throw new NotImplementedException("Save schemes is not implemented");
        // }

        // Method to save game data to a JSON file
        private static async UniTask SavePlayerData(PlayerData data)
        {
            string json = JsonUtility.ToJson(data);
            await File.WriteAllTextAsync(SavePath, json);
            Debug.Log(SavePath);
        }
        
        // Method to load game data from a JSON file
        private static async UniTask<PlayerData> LoadPlayerData()
        {
            if (File.Exists(SavePath))
            {
                string json = await File.ReadAllTextAsync(SavePath);
                return JsonConvert.DeserializeObject<PlayerData>(json);
            }

            Debug.LogWarning("Save file not found.");
            var playerData = new PlayerData();
            playerData.schemes = new List<Scheme>();
            return playerData;
        }
    }
}