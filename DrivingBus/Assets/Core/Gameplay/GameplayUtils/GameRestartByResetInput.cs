using Core.Mediators;
using Core.Services;
using UnityEngine;
using Zenject;

namespace Core.Gameplay.GameplayUtils
{
    public class GameRestartByResetInput : MonoBehaviour
    {
        [Inject] InputService _inputService;
        [Inject] GameplayMediator _gameplayMediator;

        void Awake()
        {
            _inputService.Gameplay.Reset.performed += ResetLevel_handler;
        }

        void OnDestroy()
        {
            _inputService.Gameplay.Reset.performed -= ResetLevel_handler;
        }

        void ResetLevel_handler()
        {
            _gameplayMediator.RestartLevel();
        }
    }
}