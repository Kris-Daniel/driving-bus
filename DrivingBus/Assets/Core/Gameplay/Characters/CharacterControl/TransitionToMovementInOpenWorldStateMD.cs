using Core.Gameplay.EntityBasedLogic;
using Core.Gameplay.Vehicles;
using Core.Services;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Core.Gameplay.Characters.CharacterControl
{
    public class TransitionToMovementInOpenWorldStateMD : MonoDependency
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
            Debug.Log("Entering TransitionToMovementInOpenWorldStateMD");
            _inputService.Gameplay.EnterExitCar.performed += OnExitCar;
        }

        public override void Exit()
        {
            Debug.Log("Exiting TransitionToMovementInOpenWorldStateMD");
            _inputService.Gameplay.EnterExitCar.performed -= OnExitCar;
        }

        void OnExitCar()
        {
            Debug.Log("Exiting Car");
            var car = FindAnyObjectByType<VehicleBase>();
            car.GetComponent<MonoDependencyStateController>().SetState(VehicleStateConstants.NoDriving);

            _rigidbody.isKinematic = false;
            foreach (var childCollider in _colliders)
            {
                childCollider.enabled = true;
            }
            
            transform.position = car.ExitCarPosition.position;
            transform.rotation = car.ExitCarPosition.rotation;
            
            SetterForCharacterState.SetState(CharacterStateConstants.MoveInOpenWorld);
        }
    }
}