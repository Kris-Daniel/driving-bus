using System.Collections.Generic;
using UnityEngine;

namespace Core.Services.GameResourcesSystem
{
	[CreateAssetMenu(fileName = "ResourcesToLoadSO", menuName = "Architecture/ResourcesToLoadSO", order = 0)]
	public class ResourcesToLoadSO : ScriptableObject
	{
		public List<SceneResource> Scenes;
		public List<PrefabResource> Prefabs;
	}
}