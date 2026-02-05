using Core.Mediators;
using Core.Services;
using Core.Services.Cameras;
using UnityEngine;
using Zenject;

namespace Core.Installers
{
    public class GameplayInstaller : MonoInstaller
    {
        [SerializeField] GameplayMediator _gameplayMediator;
        [SerializeField] CameraService _cameraService;

        public override void InstallBindings()
        {
            Container.Bind<GameplayMediator>().FromInstance(_gameplayMediator).AsSingle();
            
            Container.Bind<CameraService>().FromInstance(_cameraService).AsSingle();
			
            var factoryInjector = Container.Resolve<FactoryInjector>();
            factoryInjector.UpdateContainer(Container);
        }
    }
}