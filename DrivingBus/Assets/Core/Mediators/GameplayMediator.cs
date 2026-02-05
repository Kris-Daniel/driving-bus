using Core.Boot.MonoFlows;
using Core.Gameplay.GameSetups;
using UnityEngine;

namespace Core.Mediators
{
    public class GameplayMediator : MonoBehaviour
    {
        [SerializeField] GameplayMonoFlow _gameplayMonoFlow;
		
        [SerializeField] GameLevelSetup _gameplayLevelHandler;
        
        public void RestartLevel() => _gameplayLevelHandler.RestartLevel();
    }
}