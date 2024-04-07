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
        }

        private async void Init()
        {
            Scheme[] loadedSchemes = await SchemesLoader.LoadSchemes();
            _schemesContainer.AddSchemes(loadedSchemes);
        }

        private void InitializeContainers()
        {
            _schemesContainer = new SchemesContainer();
        }
    }
}