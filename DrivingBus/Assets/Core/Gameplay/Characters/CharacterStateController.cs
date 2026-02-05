using Core.Boot.FlowInterfaces;
using Core.Gameplay.Characters.CharacterControl;
using Core.Gameplay.EntityBasedLogic;
using UnityEngine;

namespace Core.Gameplay.Characters
{
    public class CharacterStateController : MonoBehaviour, IResettable
    {
        [SerializeField] MonoDependencyStateController _stateController;
        
        [SerializeField] DrivingMD _drivingMD;
        [SerializeField] MovementInOpenWorldMD _movementInOpenWorld;
        
        [SerializeField] TransitionToDrivingStateMD _transitionToDriving;
        [SerializeField] TransitionToMovementInOpenWorldStateMD _transitionToMovementInOpenWorld;
        
        [SerializeField] CharacterGroundAirMD _groundAirMD;
        [SerializeField] CharacterJumpMD _jumpMD;
        
        [SerializeField] CharacterInputsMD _inputsMD;
        [SerializeField] LookAroundInOpenWorld _lookAroundInOpenWorld;

        void Awake()
        {
            _stateController.AddStateMD(CharacterStateConstants.MoveInOpenWorld, _groundAirMD);
            _stateController.AddStateMD(CharacterStateConstants.MoveInOpenWorld, _inputsMD);
            _stateController.AddStateMD(CharacterStateConstants.MoveInOpenWorld, _jumpMD);
            _stateController.AddStateMD(CharacterStateConstants.MoveInOpenWorld, _movementInOpenWorld);
            _stateController.AddStateMD(CharacterStateConstants.MoveInOpenWorld, _transitionToDriving);
            _stateController.AddStateMD(CharacterStateConstants.MoveInOpenWorld, _lookAroundInOpenWorld);
            
            _stateController.AddStateMD(CharacterStateConstants.Driving, _inputsMD);
            _stateController.AddStateMD(CharacterStateConstants.Driving, _drivingMD);
            _stateController.AddStateMD(CharacterStateConstants.Driving, _transitionToMovementInOpenWorld);
            _stateController.AddStateMD(CharacterStateConstants.Driving, _lookAroundInOpenWorld);

            _stateController.SetState(CharacterStateConstants.MoveInOpenWorld);
        }

        public void ResetFull()
        {
            _stateController.SetState(CharacterStateConstants.MoveInOpenWorld);
        }
    }
}