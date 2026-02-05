using System;
using Core.Boot.GlobalStateMachine;
using Core.Services;
using Core.Services.GameResourcesSystem;
using Core.Utils.StateSystem;
using Core.Utils.StateSystem.Interfaces;
using UnityEngine;
using Zenject;

namespace Core.Boot
{
    public enum InstanceWasLoaded
    {
        NotLoaded,
        Loaded,
        DestroyedStatic
    }
	
    public class AppInitializer : MonoBehaviour
    {
        public static InstanceWasLoaded InstanceWasLoaded { get; private set; } = InstanceWasLoaded.NotLoaded;

        StateMachine _globalStateMachine;
		
        [Inject] IGameResourcesService _gameResourcesService;
        [Inject] IAssetLoaderService _assetLoaderService;
        [Inject] IFadeService _fadeService; 
        [Inject] InputService _inputService;
		
        void Awake()
        {
            Application.targetFrameRate = 60;
			
            InstanceWasLoaded = InstanceWasLoaded.Loaded;
			
            DontDestroyOnLoad(gameObject);
			
            _globalStateMachine = new StateMachine(new IBaseState[]
            {
                new InitState(_fadeService),
                new MainMenuState(_gameResourcesService, _assetLoaderService),
                new GameplayState(_gameResourcesService, _assetLoaderService)
            });
			
            _globalStateMachine.Enter<InitState>();
        }

        void OnDestroy()
        {
            InstanceWasLoaded = InstanceWasLoaded.DestroyedStatic;
        }
    }
}