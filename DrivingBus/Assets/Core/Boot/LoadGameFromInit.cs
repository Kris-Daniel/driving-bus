using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Core.Boot
{
    public class LoadGameFromInit : MonoBehaviour
    {
        [SerializeField] SceneContext _sceneContext;
        [SerializeField] List<string> _parentContractNames;
		
        void Awake()
        {
            if (AppInitializer.InstanceWasLoaded == InstanceWasLoaded.NotLoaded || AppInitializer.InstanceWasLoaded == InstanceWasLoaded.DestroyedStatic)
            {
                var sceneContext = FindAnyObjectByType<SceneContext>();
                if (sceneContext)
                {
                    DestroyImmediate(sceneContext.gameObject);
                }
				
                foreach (var rootGameObject in SceneManager.GetActiveScene().GetRootGameObjects())
                {
                    if (rootGameObject != gameObject)
                    {
                        DestroyImmediate(rootGameObject.gameObject);
                    }
                }
				
                SceneManager.LoadScene(0);
            }
            else
            {
                SetParentContractNamesToSceneContextViaReflection();
            }
        }

        void SetParentContractNamesToSceneContextViaReflection()
        {
            if (_sceneContext && _parentContractNames.Count > 0)
            {
                if (_sceneContext)
                {
                    var sceneContextType = _sceneContext.GetType();
                    var parentContractNamesField = sceneContextType.GetField("_parentContractNames", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    if (parentContractNamesField != null)
                    {
                        parentContractNamesField.SetValue(_sceneContext, _parentContractNames);
                    }
                }
            }
        }
    }
}