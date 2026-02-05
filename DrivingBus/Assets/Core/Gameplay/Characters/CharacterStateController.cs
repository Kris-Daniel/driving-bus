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

        void Awake()
        {
            _stateController.AddStateMD(ECharacterState.MoveInOpenWorld, _groundAirMD);
            _stateController.AddStateMD(ECharacterState.MoveInOpenWorld, _inputsMD);
            _stateController.AddStateMD(ECharacterState.MoveInOpenWorld, _jumpMD);
            _stateController.AddStateMD(ECharacterState.MoveInOpenWorld, _movementInOpenWorld);
            _stateController.AddStateMD(ECharacterState.MoveInOpenWorld, _transitionToDriving);
            
            _stateController.AddStateMD(ECharacterState.Driving, _inputsMD);
            _stateController.AddStateMD(ECharacterState.Driving, _drivingMD);
            _stateController.AddStateMD(ECharacterState.Driving, _transitionToMovementInOpenWorld);

            _stateController.SetState(ECharacterState.MoveInOpenWorld);
        }

        public void ResetFull()
        {
            _stateController.SetState(ECharacterState.MoveInOpenWorld);
        }
    }
}