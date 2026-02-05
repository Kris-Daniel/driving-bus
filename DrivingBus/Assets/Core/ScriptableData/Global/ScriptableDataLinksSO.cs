using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.ScriptableData.Global
{
    public enum EDataPathKey
    {
        GameLevels,
    }

    [Serializable]
    public class KeysWithDataPath
    {
        public EDataPathKey Key;
        public ScriptableObject ScriptableObject;
    }
	
    [CreateAssetMenu(fileName = "PrefabLinksSO", menuName = "Global/PrefabLinksSO", order = -10000)]
    public class ScriptableDataLinksSO : ScriptableObject
    {
        public List<KeysWithDataPath> DataPaths;
		
        public T GetDataByKey<T>(EDataPathKey key) where T : ScriptableObject
        {
            foreach (var dataPath in DataPaths)
            {
                if (dataPath.Key == key && dataPath.ScriptableObject is T dataToReturn)
                {
                    return dataToReturn;
                }
            }

            return null;
        }
    }
}