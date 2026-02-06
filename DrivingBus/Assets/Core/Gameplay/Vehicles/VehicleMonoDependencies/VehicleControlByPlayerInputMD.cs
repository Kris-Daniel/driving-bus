using Core.Gameplay.EntityBasedLogic;
using Core.Services;
using UnityEngine;
using Zenject;

namespace Core.Gameplay.Vehicles.VehicleMonoDependencies
{
    public class VehicleControlByPlayerInputMD : MonoDependency
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
        }

        public override void Exit()
        {
            _isInState = false;
        }
        
        void Update()
        {
            if (_isInState)
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
    }
}