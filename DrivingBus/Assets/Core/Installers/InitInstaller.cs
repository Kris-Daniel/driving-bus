using Core.Services;
using UnityEngine;
using Zenject;

namespace Core.Installers
{
    public class InitInstaller : MonoInstaller
    {
        [SerializeField] InputService _inputService;
        
        public override void InstallBindings()
        {
            _inputService.Init();
            ProjectContext.Instance.Container.Bind<InputService>().FromInstance(_inputService).AsSingle();
        }
    }
}