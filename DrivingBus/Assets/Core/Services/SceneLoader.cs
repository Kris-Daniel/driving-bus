using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.Utils.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core.Services
{
    public interface ISceneLoaderService
    {
        Task<SceneLoadResult> LoadSceneAsync(string sceneName, FullLoadSceneParameters parameters);
        Task UnloadSceneAsync(Scene scene);
    }
    
    [Serializable]
    public struct FullLoadSceneParameters
    {
        public bool IsActiveScene;
        public LoadSceneParameters LoadSceneParameters;
    }

	public class SceneLoaderService : ISceneLoaderService
	{
		readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        readonly Dictionary<string, Scene> _loadedScenes = new Dictionary<string, Scene>();
        
        public async Task<SceneLoadResult> LoadSceneAsync(string sceneName, FullLoadSceneParameters fullParameters)
        {
            using (await _semaphore.UseWaitAsync())
            {
                var tcs = new TaskCompletionSource<SceneLoadResult>();
                var time = Time.realtimeSinceStartup;

                if (_loadedScenes.ContainsKey(sceneName))
                {
                    tcs.TrySetResult(new SceneLoadResult()
                    {
                        Scene = _loadedScenes[sceneName],
                        LoadedTime = Time.realtimeSinceStartup - time,
                        Success = true,
                        AlreadyLoaded = true
                    });
                }
                else
                {
                    var asyncOperation = SceneManager.LoadSceneAsync(sceneName, fullParameters.LoadSceneParameters);
                    asyncOperation.completed += result =>
                    {
                        var scene = SceneManager.GetSceneByName(sceneName);
                        if (fullParameters.IsActiveScene)
                        {
                            SceneManager.SetActiveScene(scene);
                        }
                        _loadedScenes[sceneName] = scene;
                        tcs.TrySetResult(new SceneLoadResult()
                        {
                            Scene = scene,
                            LoadedTime = Time.realtimeSinceStartup - time,
                            Success = true,
                            AlreadyLoaded = false
                        });
                    };
                }

                return await tcs.Task;
            }
        }

        public async Task UnloadSceneAsync(Scene scene)
        {
            using (await _semaphore.UseWaitAsync())
            {
                try
                {
                    var tcs = new TaskCompletionSource<bool>();

                    var sceneName = scene.name;
                    var asyncOperation = SceneManager.UnloadSceneAsync(scene);
                    asyncOperation.completed += _ =>
                    {
                        if (_loadedScenes.ContainsKey(sceneName))
                            _loadedScenes.Remove(sceneName);

                        tcs.TrySetResult(true);
                    };

                    await tcs.Task;
                }
                catch (Exception e)
                {
                    Debug.LogError($"Something happened during scene unloading: {e.Message}.");
                }
            }
        }
    }
    
    public struct SceneLoadResult
    {
        public Scene Scene;
        public float LoadedTime;
        public bool Success;
        public bool AlreadyLoaded;
    }

}