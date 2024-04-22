using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Misc;
using Schemes;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GameLogic
{
    public class GameManager : MonoSingleton<GameManager>
    {

        [SerializeField] private List<MonoContainer> monoContainers;
        
        [DisableInEditorMode][ShowInInspector] private List<IContainer> _runtimeContainers;
        

        private async void Start()
        {
            Init();
        }
        
        private async void Init()
        {
            InitializeContainers();
            await InitSchemesContainer();
        }

        private async UniTask InitSchemesContainer()
        {
            // Note:  loading schemes. Will be probably moved to another method or even class 
            var loadedSchemes = await SchemesLoader.LoadSchemes();
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