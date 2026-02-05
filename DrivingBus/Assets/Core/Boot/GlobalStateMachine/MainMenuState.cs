using Core.Boot.MonoFlows;
using Core.ScriptableData.Global;
using Core.Services;
using Core.Services.GameResourcesSystem;
using Core.Utils.Extensions;
using Core.Utils.StateSystem;
using Cysharp.Threading.Tasks;

namespace Core.Boot.GlobalStateMachine
{
    public interface IGoToGameplay
    {
        void GoToGameplay();
    }
    
    public class MainMenuState : BaseState, IGoToGameplay
    {
        readonly IGameResourcesService _gameResourcesService;
        readonly IAssetLoaderService _assetLoaderService;
        MainMenuMonoFlow _mainMenuMonoFlow;

        public MainMenuState(IGameResourcesService gameResourcesService, IAssetLoaderService assetLoaderService)
        {
            _assetLoaderService = assetLoaderService;
            _gameResourcesService = gameResourcesService;
        }

        public override void Enter()
        {
            LoadMainMenu().GetAwaiter();
        }

        async UniTask LoadMainMenu()
        {
            var mainMenuResource = _assetLoaderService.LoadAssetByKey<GameLevelsSO>(EDataPathKey.GameLevels).MainMenuResource;
			
            await _gameResourcesService.Load(EResourceID.MainMenu, mainMenuResource);
			
            _mainMenuMonoFlow = _gameResourcesService.FindComponentResource<MainMenuMonoFlow>(EResourceID.MainMenu);
            _mainMenuMonoFlow.Init(this);
            await AsyncLib.TryCatch(_mainMenuMonoFlow.Enter());
        }

        public override void Exit()
        {
			
        }

        public void GoToGameplay()
        {
            GoToGameplayProcess().GetAwaiter();
        }

        async UniTask GoToGameplayProcess()
        {
            await _mainMenuMonoFlow.Exit();
            StateMachine.Enter<GameplayState>();
        }
    }
}