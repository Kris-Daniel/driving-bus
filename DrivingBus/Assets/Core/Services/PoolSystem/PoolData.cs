using System;
using UnityEngine;

namespace Core.Services.PoolSystem
{
	[Serializable]
	public class PoolData<T> where T : MonoBehaviour, IPoolItem
	{
		public T Prefab;
		public int InitialCount;
	}
}