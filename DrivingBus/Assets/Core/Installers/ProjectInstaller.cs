using Core.ScriptableData.Global;
using Core.Services;
using Core.Services.GameResourcesSystem;
using UnityEngine;
using Zenject;

namespace Core.Installers
{
    public class ProjectInstaller : MonoInstaller
    {
        [SerializeField] FadeService _fadeService;
		[SerializeField] ScriptableDataLinksSO _scriptableDataLinks;
		
		public override void InstallBindings()
		{
			Container.Bind<IFadeService>().FromInstance(_fadeService);
			Container.Bind<FadeService>().FromInstance(_fadeService);
			
			var factoryInjector = new FactoryInjector(Container);
			Container.Bind<FactoryInjector>().FromInstance(factoryInjector).AsSingle();
            
			var sceneLoaderService = new SceneLoaderService();
			Container.Bind<ISceneLoaderService>().FromInstance(sceneLoaderService).AsSingle();
            
			var contentProvider = new ContentProvider();
			Container.Bind<IContentProviderService>().FromInstance(contentProvider).AsSingle();
            
			var assetLoaderService = new AssetLoaderService(_scriptableDataLinks);
			Container.Bind<IAssetLoaderService>().FromInstance(assetLoaderService).AsSingle();

			var gameResourcesService = new GameResourcesService(sceneLoaderService, contentProvider, new FactoryInjector(Container));
			Container.Bind<IGameResourcesService>().FromInstance(gameResourcesService).AsSingle();
		}
    }
}