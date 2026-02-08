using Core.Boot.FlowInterfaces;
using Core.Gameplay.EntityBasedLogic;
using Core.Services;
using UnityEngine;
using Zenject;

namespace Core.Gameplay.Vehicles.VehicleMonoDependencies
{
    public class VehicleControlByPlayerInputMD : MonoDependency, IResettable
    {
        [Inject] InputService _inputService;

        bool _isInState;
        CarInputs _carInputs;

        public override void Init()
        {
            _carInputs = GetComponent<VehicleBase>().CarInputs;
        }

        public override void Enter()
        {
            _isInState = true;
            _inputService.Gameplay.Interact.performed += OnToggleEngine;
        }

        void OnToggleEngine()
        {
            _carInputs.EngineIsOn.Value = !_carInputs.EngineIsOn.Value;
        }

        public override void Exit()
        {
            _isInState = false;
            _inputService.Gameplay.Interact.performed -= OnToggleEngine;
        }
        
        void Update()
        {
            if (_isInState && _carInputs.EngineIsOn.Value)
            {
                var moveValue = _inputService.Gameplay.MoveValue();
                var moveRawValue = _inputService.Gameplay.MoveRawValue();
			    
                _carInputs.ForwardMoveAxis = Mathf.Abs(moveValue.y);
                _carInputs.ForwardMoveRaw = moveRawValue.y;
                _carInputs.TurnAxis = moveValue.x;
                _carInputs.TurnAxisRaw = moveRawValue.x;
                _carInputs.IsHandbrake = _inputService.Gameplay.Jump.IsPressed();
                _carInputs.IsNitro = _inputService.Gameplay.Sprint.IsPressed();
            }
        }

        public void ResetFull()
        {
            _carInputs.ClearInputs();
        }
    }
}