using Core.Gameplay.EntityBasedLogic;
using Core.ScriptableData;
using Core.Services;
using Core.Services.Cameras;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Core.Gameplay.Characters.CharacterControl
{
    public class CharacterInputsMD : MonoDependency
    {
        [SerializeField] CharacterControlDataSO _controlDataSO;
        
        [Inject] InputService _inputService;
        [Inject] CameraService _cameraService;
		
        Transform _cameraTransform;
        Vector3 _moveDirection;
		
        float _jumpInputBufferDelta = -1f;
        bool _isJumpInput;

        public bool IsJumpPressedAndBuffered() => _jumpInputBufferDelta > 0;
        public Vector3 MoveDirection() => _moveDirection;
        public Vector3 InputRaw() => _inputService.Gameplay.MoveRawValue();
        void Start()
        {
            _cameraTransform = _cameraService.transform;
        }
		
        public void Init(CharacterControlDataSO characterControlDataSo)
        {
            _controlDataSO = characterControlDataSo;
        }

        public override void Enter()
        {
            // _inputService.Jump.started += SetJump;
        }

        public override void Exit()
        {
            // _inputService.Jump.started -= SetJump;
        }

        void Update()
        {
            HandleInputs();
        }

        void SetJump(InputAction.CallbackContext obj)
        {
            _isJumpInput = true;
        }

        void HandleInputs()
        {
            _isJumpInput = _inputService.Gameplay.Jump.IsPressed();

            var moveRawValue = _inputService.Gameplay.MoveRawValue();
            var inputMove = moveRawValue;
            var move = new Vector3(inputMove.x, 0, inputMove.y);
            move = _cameraTransform.TransformDirection(move);
            move.y = 0;
            _moveDirection = move.normalized *  moveRawValue.magnitude;
            
            if (_isJumpInput)
            {
                _jumpInputBufferDelta = _controlDataSO.JumpButtonBuffer;
            }

            if (_jumpInputBufferDelta > 0)
            {
                _jumpInputBufferDelta -= Time.deltaTime;
            }
        }
        public override void Init()
        {
            
        }
    }
}