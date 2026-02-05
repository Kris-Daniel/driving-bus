using UnityEngine;

namespace Core.Services
{
	public interface IContentProviderService
	{
		public T InstantiatePrefab<T>(T prefab) where T : MonoBehaviour;
		
		public T InstantiatePrefabInactive<T>(T prefab) where T : MonoBehaviour;

		public GameObject InstantiatePrefab(GameObject prefab);
		public GameObject InstantiatePrefabInactive(GameObject prefab);
	}

	public class ContentProvider : IContentProviderService
	{
		public T InstantiatePrefab<T>(T prefab) where T : MonoBehaviour
		{
			return Object.Instantiate(prefab);
		}
		
		public T InstantiatePrefabInactive<T>(T prefab) where T : MonoBehaviour
		{
			var tempParentPrefab = GameObject.CreatePrimitive(PrimitiveType.Quad);
			tempParentPrefab.SetActive(false);

			var typedPrefab = (T) prefab;
			var instantiatedComponent = Object.Instantiate(typedPrefab, tempParentPrefab.transform);
			instantiatedComponent.gameObject.SetActive(false);
			instantiatedComponent.transform.SetParent(null);
			
			Object.Destroy(tempParentPrefab);
			
			return instantiatedComponent;
		}

		public GameObject InstantiatePrefab(GameObject prefab)
		{
			return Object.Instantiate(prefab);
		}

		public GameObject InstantiatePrefabInactive(GameObject prefab)
		{
			var tempParentPrefab = GameObject.CreatePrimitive(PrimitiveType.Quad);
			tempParentPrefab.SetActive(false);

			var instantiatedComponent = Object.Instantiate(prefab, tempParentPrefab.transform);
			instantiatedComponent.gameObject.SetActive(false);
			instantiatedComponent.transform.SetParent(null);
			
			Object.Destroy(tempParentPrefab);
			
			return instantiatedComponent;
		}
	}
}