using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core.Services.GameResourcesSystem
{
	public class LoadedResources
	{
		public List<LoadedSceneResource> Scenes = new List<LoadedSceneResource>();
		public List<LoadedGameObjectResource> GameObjects = new List<LoadedGameObjectResource>();
	}

	public class LoadedSceneResource
	{
		public Scene Scene;
	}

	public class LoadedGameObjectResource
	{
		public GameObject GameObject;
	}
}