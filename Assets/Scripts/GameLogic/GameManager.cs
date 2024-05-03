using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Misc;
using Schemes;
using Schemes.Dashboard;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace GameLogic
{
    public class GameManager : MonoSingleton<GameManager>
    {
        [SerializeField] private SchemesModelCapture schemesModelCapture;
        [SerializeField] private List<MonoContainer> monoContainers;
        
        [DisableInPlayMode][DisableInEditorMode][ShowInInspector] private List<IContainer> _runtimeContainers;
        
        private async void Start()
        {
            Init();
            Application.targetFrameRate = 120;
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
        
        private async void Init()
        {
            InitializeContainers();
            await InitSchemesContainer();
            InitDashboard();
        }

        private void InitDashboard()
        {
            EditorDashboard.Instance.Init();
        }

        private async UniTask InitSchemesContainer()
        {
            // Note:  loading schemes. Will be probably moved to another method or even class 
            var loadedSchemes = await SchemesSaverLoader.LoadSchemes();
            
            schemesModelCapture.CaptureSchemesRenderTextures(loadedSchemes);
            SchemesContainer schemesContainer = new SchemesContainer();
            schemesContainer.AddSchemes(loadedSchemes);
            _runtimeContainers.Add(schemesContainer);
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