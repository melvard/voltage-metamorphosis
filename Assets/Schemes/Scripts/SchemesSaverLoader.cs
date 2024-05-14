using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Canvas;
using Cysharp.Threading.Tasks;
using GameLogic;
using Misc;
using Newtonsoft.Json;
using Schemes.Data;
using UnityEngine;
using UnityEngine.Events;

namespace Schemes
{
    [Serializable]
    public struct PlayerData
    {
        public List<Scheme> schemes;
    }
    
    public static class SchemesSaverLoader
    {
        #region PRIVATE_FIELDS

        private static readonly string SavePath = Path.Combine(Application.persistentDataPath, "PlayerData.json");
        private static PlayerData PlayerData;

        #endregion
        
        #region EVENTS

        public static event UnityAction<SchemeInteractionEventArgs> OnSchemeRemoved; 
        public static event UnityAction<SchemeInteractionEventArgs> OnSchemeAdded; 
        public static event UnityAction<SchemeInteractionEventArgs> OnSchemeEdited; 

        #endregion
        
        public static void RemoveEventSubscriptions()
        {
            OnSchemeRemoved = null;
            OnSchemeAdded = null;
            OnSchemeEdited = null;
        }
        private static List<Scheme> GetDefaultSchemes()
        {
            var schemeDatas = GameManager.Instance.GetContainerOfType<ConfigsContainer>()
                .DefaultScriptableSchemesSo
                .DefaultSchemesDataList;

            return schemeDatas.Select(schemeData => new Scheme(schemeData)).ToList();
        }
        
        public static async UniTask<List<Scheme>> LoadSchemes(CancellationToken ct)
        {
            var defaultSchemes = GetDefaultSchemes();
            var playerSchemes = await GetPlayerSchemes(ct);
            
            if (playerSchemes == null) return defaultSchemes;
            return defaultSchemes.Union(playerSchemes).ToList();
        }
        
        private static async UniTask<List<Scheme>> GetPlayerSchemes(CancellationToken ct)
        {
            PlayerData = await LoadPlayerData(ct);
            return PlayerData.schemes;
        }
        
        // // Note: will not be used probably
        // public static async UniTask<Scheme> LoadSchemeByName()
        // {
        //     // todo: deserialization from json file here 
        //     throw new NotImplementedException("Load scheme by name is not implemented");
        // }
        //
        // public static async UniTask<Scheme> LoadSchemeByKey(SchemeKey guidStr)
        // {
        //     // todo: deserialization from json file here 
        //     throw new NotImplementedException("Load scheme by key is not implemented");
        // }

        public static async UniTask SaveScheme(Scheme scheme, CancellationToken ct)
        {
            var previousSameSchemeIndex = PlayerData.schemes.IndexOf(x => x == scheme);
            if (previousSameSchemeIndex != -1)
            {
                PlayerData.schemes[previousSameSchemeIndex] = scheme;
                OnSchemeEdited?.Invoke(new SchemeInteractionEventArgs(scheme));
            }
            else
            {
                PlayerData.schemes.Add(scheme);
                OnSchemeAdded?.Invoke(new SchemeInteractionEventArgs(scheme));
            }

            await SavePlayerData(PlayerData, ct);
        }

        // public static async UniTask<bool> SaveSchemes(List<Scheme> schemes)
        // {
        //     throw new NotImplementedException("Save schemes is not implemented");
        // }

        // Method to save game data to a JSON file
        private static async UniTask SavePlayerData(PlayerData data, CancellationToken ct)
        {
            string json = JsonUtility.ToJson(data);
            await File.WriteAllTextAsync(SavePath, json, cancellationToken:ct); // fixme where is your cancellation token
            Debug.Log(SavePath);
        }
        
        // Method to load game data from a JSON file
        private static async UniTask<PlayerData> LoadPlayerData(CancellationToken ct)
        {
            if (File.Exists(SavePath))
            {
                string json = await File.ReadAllTextAsync(SavePath, cancellationToken:ct);
                return JsonUtility.FromJson<PlayerData>(json);
            }

            Debug.LogWarning("Save file not found.");
            var playerData = new PlayerData();
            playerData.schemes = new List<Scheme>();
            return playerData;
        }

        public static async void OnRemoveSchemeHandler(SchemeInteractionEventArgs removeSchemeHandler, CancellationToken ct)
        {
            var schemeToRemoveIndex = PlayerData.schemes.IndexOf(x => x == removeSchemeHandler.scheme);
            if (schemeToRemoveIndex != -1)
            {
                PlayerData.schemes.RemoveAt(schemeToRemoveIndex);
            }
            await SavePlayerData(PlayerData, ct);
            OnSchemeRemoved?.Invoke(new SchemeInteractionEventArgs(removeSchemeHandler.scheme));
        }
    }
}