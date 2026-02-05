using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Core.Services.GameResourcesSystem
{
	public interface IGameResourcesService
	{
		Task Load(EResourceID resourceID, ResourcesToLoadSO resourcesToLoadSo);

		Task Unload(EResourceID resourceID);

		Task UnloadAll();
		
		T FindComponentResource<T>(EResourceID resourceID) where T : MonoBehaviour;
		List<T> FindInstancesFromResource<T>(EResourceID resourceID);
	}
}