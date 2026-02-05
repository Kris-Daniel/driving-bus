using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Services.Cameras
{
    public class CameraService : MonoBehaviour
    {
        [SerializeField] Camera _mainCamera;
		
        public Camera MainCamera => _mainCamera;

        public void Init()
        {
            
        }
    }
}