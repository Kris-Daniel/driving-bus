using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Core.Services.PoolSystem
{
	public interface IPool
	{
		void ReturnObject(IPoolItem item);
	}

	[Serializable]
	public class Pool<T> : IPool where T : MonoBehaviour, IPoolItem
	{
		public PoolData<T> _poolData;

		List<T> _storedItems = new List<T>();
		List<T> _spawnedItems = new List<T>();
		IContentProviderService _contentProvider;
		Transform _poolManager;
		FactoryInjector _factoryInjector;

		Vector3 _initialScale;

		public void InitPool(Transform poolManager, IContentProviderService contentProvider, FactoryInjector factoryInjector)
		{
			_poolManager = poolManager;
			_contentProvider = contentProvider;
			_factoryInjector = factoryInjector;
			_initialScale = _poolData.Prefab.transform.localScale;
			for (var i = 0; i < _poolData.InitialCount; i++)
			{
				InstantiateItem();
			}
		}

		T InstantiateItem()
		{
			var instantiatedItem = _contentProvider.InstantiatePrefabInactive(_poolData.Prefab);
			_factoryInjector.InjectGameObject(instantiatedItem.gameObject);
			instantiatedItem.OnSetPool(this);
			instantiatedItem.transform.SetParent(_poolManager);
			_storedItems.Add(instantiatedItem);
			
			return instantiatedItem;
		}

		public T SpawnItemInactive()
		{
			if (_storedItems.Count == 0)
			{
				InstantiateItem();
			}
			
			var item = _storedItems[0];
			_storedItems.RemoveAt(0);
			_spawnedItems.Add(item);

			item.transform.localScale = _initialScale;
			return item;
		}

		public T SpawnItem()
		{
			var item = SpawnItemInactive();
			item.gameObject.SetActive(true);
			return item;
		}
		
		public T SpawnItemInside(Transform parent)
		{
			var item = SpawnItemInactive();
			item.transform.SetParent(parent);
			item.transform.localPosition = Vector3.zero;
			item.transform.localRotation = Quaternion.identity;
			item.gameObject.SetActive(true);
			return item;
		}

		public void ReturnObject(IPoolItem item)
		{
			var castedItem = (T)item;
			_spawnedItems.Remove(castedItem);
			_storedItems.Add(castedItem);
			
			castedItem.gameObject.SetActive(false);
			castedItem.transform.SetParent(_poolManager);
			
			item.AfterReturnToPool();
		}

		public void ReturnAllObjects()
		{
			var spawnedItemsCount = _spawnedItems.Count;
			for (int i = 0; i < spawnedItemsCount; i++)
			{
				ReturnObject(_spawnedItems[0]);
			}

			for (int i = _storedItems.Count - 1; i >= _poolData.InitialCount; i--)
			{
				Object.Destroy(_storedItems[i].gameObject);
				_storedItems.RemoveAt(i);
			}
		}

		public List<T> GetSpawnedItems()
		{
			return new List<T>(_spawnedItems);
		}
	}

}