using UnityEngine;

namespace Core.Services.Cameras
{
    public class CameraService : MonoBehaviour
    {
        [SerializeField] Camera _mainCamera;
        [SerializeField] Transform _walkingCamera;
        [SerializeField] Transform _drivingCamera;
        
        public Transform WalkingCamera => _walkingCamera;
        public Transform DrivingCamera => _drivingCamera;
		
        public Camera MainCamera => _mainCamera;

        public void Init()
        {
            
        }
    }
}