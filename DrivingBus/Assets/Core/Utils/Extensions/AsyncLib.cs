using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core.Utils.Extensions
{
	public static class AsyncLib
	{
		public static async UniTask<T> TryCatch<T>(UniTask<T> task)
		{
			try
			{
				return await task;
			}
			catch (Exception e)
			{
				Debug.LogError(e);
				return default;
			}
		}
		
		public static async UniTask TryCatch(UniTask task)
		{
			try
			{
				await task;
			}
			catch (Exception e)
			{
				Debug.LogError(e);
			}
		}
	}
}