using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Misc;
using Schemes;
using Schemes.Dashboard;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameLogic
{
    public class GameManager : MonoSingleton<GameManager>
    {
        #region CONSTS

        private const string LOADING_SCENE_NAME = "LoadingScreen";

        #endregion
        
        #region SERIALIZED_FIELDS

        [SerializeField] private SchemesModelCapture schemesModelCapture;
        [SerializeField] private List<MonoContainer> monoContainers;

        #endregion

        #region PRIVATE_FIELDS

        [DisableInPlayMode][DisableInEditorMode][ShowInInspector] private List<IContainer> _runtimeContainers;
        private CancellationTokenSource _initializationCancellationSource;
        
        #endregion
        
        private void Start()
        {
            _initializationCancellationSource = new CancellationTokenSource();
            Application.targetFrameRate = 120;
            Init(_initializationCancellationSource.Token);
        }
        
        
        #if UNITY_EDITOR
        
        private void Update()
        {
            if (Input.GetKeyDown("."))
            {
                Debug.Break();
            }
        }
        
        #endif
        
        private async void Init(CancellationToken ck)
        {
            InitializeContainers();
            await InitSchemesContainer(ck);
            InitDashboard();
        }

        private void InitDashboard()
        {
            EditorDashboard.Instance.Init();
        }

        private async UniTask InitSchemesContainer(CancellationToken ck)
        {
            SceneManager.LoadScene(LOADING_SCENE_NAME, LoadSceneMode.Additive);
            // Note:  loading schemes. Will be probably moved to another method or even class 
            var loadedSchemes = await SchemesSaverLoader.LoadSchemes(ck);
            await schemesModelCapture.CaptureSchemesRenderTextures(loadedSchemes, ck);
            SchemesContainer schemesContainer = new SchemesContainer();
            schemesContainer.AddSchemes(loadedSchemes);
            _runtimeContainers.Add(schemesContainer);
            SceneManager.UnloadSceneAsync(LOADING_SCENE_NAME);
        }
        
        private void InitializeContainers()
        {
            _runtimeContainers = new();
        }

        public T GetContainerOfType<T>() where T : IContainer
        {
            foreach (var regularContainer in _runtimeContainers)
            {
                if (regularContainer is T container)
                {
                    return container;
                }
            }
            
            foreach (var monoContainer in monoContainers)
            {
                if (monoContainer is T container)
                {
                    return container;
                }
            }

            throw new InvalidOperationException($"Can't get container of type {typeof(T)}");
        }
    }
}