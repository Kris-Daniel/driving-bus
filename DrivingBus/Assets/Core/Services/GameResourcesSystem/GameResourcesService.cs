using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Core.Services.GameResourcesSystem
{
	public class GameResourcesService : IGameResourcesService
	{
		readonly Dictionary<EResourceID, LoadedResources> _loadedResources = new Dictionary<EResourceID, LoadedResources>();
		readonly ISceneLoaderService _sceneLoaderService;
		readonly IContentProviderService _contentProviderService;
		readonly FactoryInjector _factoryInjector;

		public GameResourcesService(ISceneLoaderService sceneLoaderService, IContentProviderService contentProviderService, FactoryInjector factoryInjector)
		{
			_factoryInjector = factoryInjector;
			_contentProviderService = contentProviderService;
			_sceneLoaderService = sceneLoaderService;
		}

		// TODO: Add FadeService based on Load and Unload methods
		public async Task Load(EResourceID resourceID, ResourcesToLoadSO resourcesToLoadSo)
		{
			if(_loadedResources.ContainsKey(resourceID)) return;
			
			_loadedResources[resourceID] = new LoadedResources();
			
			foreach (var sceneResource in resourcesToLoadSo.Scenes)
			{
				var scene = await _sceneLoaderService.LoadSceneAsync(sceneResource.SceneName, sceneResource.LoadSceneParameters);
				_loadedResources[resourceID].Scenes.Add(new LoadedSceneResource()
				{
					Scene = scene.Scene
				});
			}

			try
			{
				foreach (var prefabResource in resourcesToLoadSo.Prefabs)
				{
					GameObject instantiatedPrefab = null;
				
					instantiatedPrefab = _contentProviderService.InstantiatePrefab(prefabResource.Prefab);
					_factoryInjector.InjectGameObject(instantiatedPrefab);
				
					_loadedResources[resourceID].GameObjects.Add(new LoadedGameObjectResource()
					{
						GameObject = instantiatedPrefab
					});
				}
			}
			catch (Exception e)
			{
				Debug.LogError(e);
				throw;
			}
		}

		public async Task Unload(EResourceID resourceID)
		{
			if(!_loadedResources.ContainsKey(resourceID)) return;
			
			foreach (var loadedGameObjectResource in _loadedResources[resourceID].GameObjects)
			{
				Object.Destroy(loadedGameObjectResource.GameObject);
			}

			foreach (var loadedSceneResource in _loadedResources[resourceID].Scenes)
			{
				await _sceneLoaderService.UnloadSceneAsync(loadedSceneResource.Scene);
			}
			
			_loadedResources.Remove(resourceID);
		}

		public async Task UnloadAll()
		{
			var keys = _loadedResources.Keys.ToList();
			for (var i = keys.Count - 1; i >= 0; i--)
			{
				await Unload(keys[i]);
			}
		}

		public T FindComponentResource<T>(EResourceID resourceID) where T : MonoBehaviour
		{
			if(!_loadedResources.ContainsKey(resourceID)) return null;
			
			foreach (var scenes in _loadedResources[resourceID].Scenes)
			{
				foreach (var rootGameObject in scenes.Scene.GetRootGameObjects())
				{
					var component = rootGameObject.GetComponentInChildren<T>();
					
					if(component != null)
						return component;
				}
			}
			
			foreach (var loadedGameObjectResource in _loadedResources[resourceID].GameObjects)
			{
				var component = loadedGameObjectResource.GameObject.GetComponentInChildren<T>();
				if(component != null)
					return component;
			}

			return null;
		}

		public List<T> FindInstancesFromResource<T>(EResourceID resourceID)
		{
			var components = new List<T>();
			
			if(!_loadedResources.ContainsKey(resourceID)) return components;
			
			foreach (var scenes in _loadedResources[resourceID].Scenes)
			{
				foreach (var rootGameObject in scenes.Scene.GetRootGameObjects())
				{
					components.AddRange(rootGameObject.GetComponentsInChildren<T>(true));
				}
			}
			
			foreach (var loadedGameObjectResource in _loadedResources[resourceID].GameObjects)
			{
				components.AddRange(loadedGameObjectResource.GameObject.GetComponentsInChildren<T>(true));
			}

			return components;
		}
	}
}