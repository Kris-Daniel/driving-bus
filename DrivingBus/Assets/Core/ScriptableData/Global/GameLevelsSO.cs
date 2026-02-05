using Core.Services.GameResourcesSystem;
using UnityEngine;

namespace Core.ScriptableData.Global
{
    
    [CreateAssetMenu(fileName = "GameLevelsSO", menuName = "Global/GameLevelsSO")]
    public class GameLevelsSO : ScriptableObject
    {
        public ResourcesToLoadSO MainMenuResource;
        public ResourcesToLoadSO GameplayResource;
    }
}