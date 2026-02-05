using Core.Boot.FlowInterfaces;
using Core.Services;
using Core.Services.GameResourcesSystem;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Zenject;

namespace Core.Gameplay.GameSetups
{
    public class GameLevelSetup : MonoBehaviour
    {
        [Inject] IGameResourcesService _gameResourcesService;
        [Inject] InputService _inputService;
        [Inject] IFadeService _fadeService;
        
        public async UniTask RunSetup()
        {
            /*var playerSpawner = FindAnyObjectByType<PlayerSpawner>(FindObjectsInactive.Exclude);
			
            var player = playerSpawner.SpawnPlayer();
			
            var carSpawners = FindObjectsByType<CarSpawner>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
			
            foreach (var carSpawner in carSpawners)
            {
                carSpawner.Activate();
            }*/
			
            // _gameplayLevelHandler.StartLevel();
			
            await UniTask.WaitForEndOfFrame();
            await ResetLevel();
        }

        public void RestartLevel()
        {
            ResetLevel().GetAwaiter();
        }
        
        async UniTask ResetLevel()
        {
            StopGameplayInput();
            await _fadeService.FadeInTween().AsyncWaitForCompletion();;
			
            var resettables = _gameResourcesService.FindInstancesFromResource<IResettable>(EResourceID.Gameplay);
            foreach (var resettable in resettables)
            {
                resettable.ResetFull();
            }

            StartLevel();
            await _fadeService.FadeOutTween().AsyncWaitForCompletion();;
			
            StartGameplayInput();
        }
        
        void StopGameplayInput()
        {
            _inputService.Gameplay.Deactivate();
        }

        void StartGameplayInput()
        {
            _inputService.Gameplay.Activate();
        }
        
        void StartLevel()
        {
            var onGameplayStartList = _gameResourcesService.FindInstancesFromResource<IOnGameplayStart>(EResourceID.Gameplay);
            foreach (var onGameplayStart in onGameplayStartList)
            {
                onGameplayStart.OnGameplayStart();
            }
        }
    }
}