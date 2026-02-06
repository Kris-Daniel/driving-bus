using Core.Gameplay.EntityBasedLogic;
using Core.Gameplay.Vehicles;
using Core.Services;
using UnityEngine;
using Zenject;

namespace Core.Gameplay.Characters.CharacterControl
{
    public class TransitionToDrivingStateMD : MonoDependency
    {
        [Inject] InputService _inputService;
        
        Rigidbody _rigidbody;
        Collider[] _colliders;

        public override void Init()
        {
            _rigidbody =  GetComponent<Rigidbody>();
            _colliders = _rigidbody.GetComponentsInChildren<Collider>();
        }

        public override void Enter()
        {
            Debug.Log("Entering TransitionToDrivingStateMD");
            _inputService.Gameplay.Interact.performed += OnGoToDrivingState;
        }

        public override void Exit()
        {
            Debug.Log("Exiting TransitionToDrivingStateMD");
            _inputService.Gameplay.Interact.performed -= OnGoToDrivingState;
        }

        void OnGoToDrivingState()
        {
            Debug.Log("Transitioning to driving state");
            var car = FindAnyObjectByType<VehicleBase>();
            car.GetComponent<MonoDependencyStateController>().SetState(VehicleStateConstants.DriveByPlayer);
            
            _rigidbody.isKinematic = true;
            foreach (var childCollider in _colliders)
            {
                childCollider.enabled = false;
            }
            
            transform.SetParent(car.DriverPosition);

            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            
            SetterForCharacterState.SetState(CharacterStateConstants.Driving);
        }
    }
}