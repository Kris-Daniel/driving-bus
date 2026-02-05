using Core.Boot.MonoFlows;
using Core.ScriptableData.Global;
using Core.Services;
using Core.Services.GameResourcesSystem;
using Core.Utils.Extensions;
using Core.Utils.StateSystem;
using Cysharp.Threading.Tasks;

namespace Core.Boot.GlobalStateMachine
{
    public interface IGoToMainMenu
    {
        void GoToMainMenu();
    }
    
    public class GameplayState : BaseState, IGoToMainMenu
    {
        readonly IGameResourcesService _gameResourcesService;
        IAssetLoaderService _assetLoaderService;
        GameplayMonoFlow _gameplayMonoFlow;

        public GameplayState(IGameResourcesService gameResourcesService, IAssetLoaderService assetLoaderService)
        {
            _assetLoaderService = assetLoaderService;
            _gameResourcesService = gameResourcesService;
        }

        public override void Enter()
        {
            LoadOpenWorld().GetAwaiter();
        }

        async UniTask LoadOpenWorld()
        {
            var gameResource = _assetLoaderService.LoadAssetByKey<GameLevelsSO>(EDataPathKey.GameLevels).GameplayResource;
            
            await _gameResourcesService.Load(EResourceID.Gameplay, gameResource);
			
            _gameplayMonoFlow = _gameResourcesService.FindComponentResource<GameplayMonoFlow>(EResourceID.Gameplay);
            _gameplayMonoFlow.Init(this);
            await AsyncLib.TryCatch(_gameplayMonoFlow.Enter());
        }

        public override void Exit()
        {
			
        }

        public void GoToMainMenu() => GoToMainMenuProcess().GetAwaiter();

        async UniTask GoToMainMenuProcess()
        {
            await _gameplayMonoFlow.Exit();
			
            await _gameResourcesService.Unload(EResourceID.Gameplay);
			
            StateMachine.Enter<MainMenuState>();
        }
    }
}