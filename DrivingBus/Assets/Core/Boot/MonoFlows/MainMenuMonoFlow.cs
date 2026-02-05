using System.Collections;
using Core.Boot.GlobalStateMachine;
using Core.Services;
using Core.Services.GameResourcesSystem;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Zenject;

namespace Core.Boot.MonoFlows
{
    public class MainMenuMonoFlow : MonoStateFlow
    {
        [Inject] InputService _inputService;
        [Inject] IGameResourcesService _gameResourcesService;
        [Inject] IFadeService _fadeService;

        IGoToGameplay _goToGameplay;

        bool _wasInited;

        public void Init(IGoToGameplay goToGameplay)
        {
            _goToGameplay = goToGameplay;
        }
		
        public override async UniTask Enter()
        {
            _inputService.Gameplay.Deactivate();
			
            await _fadeService.FadeOutTween().AsyncWaitForCompletion();

            StartCoroutine(GoToGameplayProcess());
        }

        IEnumerator GoToGameplayProcess()
        {
            for (int i = 0; i < 4; i++)
            {
                Debug.Log($"Going to the gameplay in {3 - i} seconds");
                yield return new WaitForSeconds(1f);
            }

            GoToGameplay();
        }

        public override async UniTask Exit()
        {
            await _fadeService.FadeInTween().AsyncWaitForCompletion();
        }

        public void GoToGameplay()
        {
            _goToGameplay.GoToGameplay();
        }
    }
}