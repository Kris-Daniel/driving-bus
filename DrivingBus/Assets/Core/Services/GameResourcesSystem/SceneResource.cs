using System;
using UnityEngine.SceneManagement;

namespace Core.Services.GameResourcesSystem
{
	[Serializable]
	public class SceneResource
	{
		public string SceneName;
		public FullLoadSceneParameters LoadSceneParameters;
	}
}