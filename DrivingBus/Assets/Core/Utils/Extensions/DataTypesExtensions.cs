namespace Core.Utils.Extensions
{
    public static class DataTypesExtensions
    {
        public static bool Approximately(this float a, float b) => UnityEngine.Mathf.Approximately(a, b);
    }
}