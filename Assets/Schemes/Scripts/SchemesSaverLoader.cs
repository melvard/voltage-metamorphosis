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
using Newtonsoft.Json.Linq;
using Schemes.Data;
using Schemes.Data.Save;
using UnityEngine;
using UnityEngine.Events;

namespace Schemes
{
    public static class SchemesSaverLoader
    {
        #region PRIVATE_FIELDS

        private static readonly string SavePath = Path.Combine(Application.persistentDataPath, "PlayerData.json");
        private static readonly JsonSerializerSettings SaveJsonSettings = new() { Formatting = Formatting.Indented };
        private static List<Scheme> _playerSchemes = new();
        private static bool _playerSchemesLoaded;

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
            _playerSchemesLoaded = false;
            var save = await LoadPlayerSave(ct);
            if (save == null)
            {
                _playerSchemes = new List<Scheme>();
                _playerSchemesLoaded = true;
                return _playerSchemes;
            }

            _playerSchemes = SaveConverter.FromSaveFile(save); // throws SaveValidationException on bad data
            _playerSchemesLoaded = true;
            return _playerSchemes;
        }

        public static async UniTask SaveScheme(Scheme scheme, CancellationToken ct)
        {
            if (!_playerSchemesLoaded) await GetPlayerSchemes(ct);

            var previousSameSchemeIndex = _playerSchemes.IndexOf(x => x == scheme);
            if (previousSameSchemeIndex != -1)
            {
                _playerSchemes[previousSameSchemeIndex] = scheme;
                OnSchemeEdited?.Invoke(new SchemeInteractionEventArgs(scheme));
            }
            else
            {
                _playerSchemes.Add(scheme);
                OnSchemeAdded?.Invoke(new SchemeInteractionEventArgs(scheme));
            }

            await SavePlayerData(ct);
        }

        private static async UniTask SavePlayerData(CancellationToken ct)
        {
            var file = SaveConverter.ToSaveFile(_playerSchemes);
            SaveValidator.Validate(file);

            // Back up the previous file before overwriting it
            if (File.Exists(SavePath))
                File.Copy(SavePath, SavePath + ".bak", true);

            var json = JsonConvert.SerializeObject(file, SaveJsonSettings);
            await File.WriteAllTextAsync(SavePath, json, cancellationToken: ct);
            Debug.Log(SavePath);
        }

        private static async UniTask<SaveFileV1> LoadPlayerSave(CancellationToken ct)
        {
            if (!File.Exists(SavePath))
            {
                Debug.LogWarning("Save file not found. Starting with empty player data.");
                return new SaveFileV1();
            }

            var json = await File.ReadAllTextAsync(SavePath, cancellationToken: ct);
            if (string.IsNullOrWhiteSpace(json))
            {
                Debug.LogWarning("Save file is empty. Starting with empty player data.");
                return new SaveFileV1();
            }

            JObject root;
            try
            {
                root = JObject.Parse(json);
            }
            catch (Exception e)
            {
                throw new SaveValidationException("Save file is not valid JSON.", e);
            }

            // Legacy format (Odin rid-based graph) is identified structurally:
            // a top-level "references" object holding RefIds.
            if (root["references"] != null)
            {
                try
                {
                    var migrated = SaveMigrator.Migrate(json);
                    SaveValidator.Validate(migrated);
                    Debug.Log("Legacy save file migrated to V1 format.");
                    return migrated;
                }
                catch (Exception e)
                {
                    Debug.LogError($"Legacy save migration failed: {e.Message}");
                    throw new SaveValidationException("Could not migrate legacy save file.", e);
                }
            }

            // V1 format
            try
            {
                var v1 = root.ToObject<SaveFileV1>();
                if (v1 == null) throw new SaveValidationException("Save file deserialized to null.");
                SaveValidator.Validate(v1);
                return v1;
            }
            catch (SaveValidationException)
            {
                throw; // Do not silently swallow validation errors
            }
            catch (Exception e)
            {
                throw new SaveValidationException($"Save file could not be parsed as V1: {e.Message}", e);
            }
        }

        public static async void OnRemoveSchemeHandler(SchemeInteractionEventArgs removeSchemeHandler, CancellationToken ct)
        {
            if (!_playerSchemesLoaded) await GetPlayerSchemes(ct);

            var schemeToRemoveIndex = _playerSchemes.IndexOf(x => x == removeSchemeHandler.scheme);
            if (schemeToRemoveIndex != -1)
            {
                _playerSchemes.RemoveAt(schemeToRemoveIndex);
            }
            await SavePlayerData(ct);
            OnSchemeRemoved?.Invoke(new SchemeInteractionEventArgs(removeSchemeHandler.scheme));
        }
    }
}
