using Core.Gameplay.EntityBasedLogic;
using Core.Services;
using Core.Services.Cameras;
using UnityEngine;
using Zenject;

namespace Core.Gameplay.Characters.CharacterControl
{
    public class LookAroundInOpenWorld : MonoDependency
    {
        [SerializeField] float _mouseSensitivity = 100f;
        [SerializeField] float _maxLookAngle = 80f;
        
        [Inject] InputService _inputService;
        [Inject] CameraService _cameraService;
        
        float _xRotation = 0f;
        
        public override void Init()
        {
            
        }

        public override void Enter()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public override void Exit()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = true;
        }

        void Update()
        {
            var look = _inputService.Gameplay.LookValue();
            float mouseX = look.x * _mouseSensitivity * Time.deltaTime;
            float mouseY = look.y * _mouseSensitivity * Time.deltaTime;

            _xRotation -= mouseY;
            _xRotation = Mathf.Clamp(_xRotation, -_maxLookAngle, _maxLookAngle);
            _cameraService.WalkingCamera.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);

            transform.Rotate(Vector3.up * mouseX);
        }
    }
}