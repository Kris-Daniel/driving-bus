namespace Core.Services.PoolSystem
{
	public interface IPoolItem
	{
		void OnSetPool(IPool pool);
		void ReturnToPool();
		void AfterReturnToPool();
	}

}