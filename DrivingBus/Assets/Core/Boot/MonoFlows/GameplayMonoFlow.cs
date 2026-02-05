using Core.Boot.FlowInterfaces;
using Core.Boot.GlobalStateMachine;
using Core.Gameplay.GameSetups;
using Core.Services;
using Core.Services.GameResourcesSystem;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Zenject;

namespace Core.Boot.MonoFlows
{
    public class GameplayMonoFlow : MonoStateFlow
	{
		[SerializeField] GameLevelSetup _gameLevelSetup;
		
		[Inject] IGameResourcesService _gameResourcesService;
		[Inject] InputService _inputService;
		[Inject] IFadeService _fadeService;
		
		IGoToMainMenu _goToMaiMenu;

		public void Init(IGoToMainMenu goToMaiMenu)
		{
			_goToMaiMenu = goToMaiMenu;
		}

		public override async UniTask Enter()
		{
			_inputService.Gameplay.Deactivate();
			
			await _fadeService.FadeOutTween().AsyncWaitForCompletion();
			await _gameLevelSetup.RunSetup();
		}

		public void GoToMainMenu()
		{
			_goToMaiMenu.GoToMainMenu();
		}
		
		public override async UniTask Exit()
		{
			await _fadeService.FadeInTween().AsyncWaitForCompletion();
		}
    }
}