using Core.ScriptableData.Global;
using UnityEngine;

namespace Core.Services
{
    public interface IAssetLoaderService
    {
        public T LoadAsset<T>(string path) where T : Object;
        public T LoadAssetByKey<T>(EDataPathKey pathKey) where T : ScriptableObject;
    }

    public class AssetLoaderService : IAssetLoaderService
    {
        ScriptableDataLinksSO _scriptableDataLinks;

        public AssetLoaderService(ScriptableDataLinksSO scriptableDataLinks)
        {
            _scriptableDataLinks = scriptableDataLinks;
        }

        public T LoadAsset<T>(string path) where T : Object
        {
            return Resources.Load<T>(path);
        }
		
        public T LoadAssetByKey<T>(EDataPathKey pathKey) where T : ScriptableObject
        {
            return _scriptableDataLinks.GetDataByKey<T>(pathKey);
        }
    }
}