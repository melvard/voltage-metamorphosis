using System;
using Schemes;
using UnityEngine;

namespace GameLogic
{
    public class GameManager : MonoBehaviour
    {
        private SchemesContainer _schemesContainer;
        private async void Start()
        {
            Init();
        }
        
        private async void Init()
        {
            InitializeContainers();
            
            // Note:  loading schemes. Will be probably moved to another method or even class 
            Scheme[] loadedSchemes = await SchemesLoader.LoadSchemes();
            _schemesContainer.AddSchemes(loadedSchemes);
        }

        private void InitializeContainers()
        {
            _schemesContainer = new SchemesContainer();
        }
    }
}