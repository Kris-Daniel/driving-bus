using UnityEngine;
using Zenject;

namespace Core.Services
{
	public class FactoryInjector
	{
		DiContainer _diContainer;

		public FactoryInjector(DiContainer diContainer)
		{
			_diContainer = diContainer;
		}
		
		public void UpdateContainer(DiContainer diContainer)
		{
			_diContainer = diContainer;
		}

		public Object Inject(Object injectable)
		{
			_diContainer.Inject(injectable);
			
			return injectable;
		}
		
		public GameObject InjectGameObject(GameObject injectable)
		{
			_diContainer.InjectGameObject(injectable);
			
			return injectable;
		}

	}
}