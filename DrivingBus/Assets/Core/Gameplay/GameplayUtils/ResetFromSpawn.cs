using Core.Boot.FlowInterfaces;
using UnityEngine;

namespace Core.Gameplay.GameplayUtils
{
    public class ResetFromSpawn : MonoBehaviour, IResettable
    {
        Vector3 _spawnPos;
        Quaternion _spawnRot;
        
        Rigidbody _rigidbody;
        
        void Awake()
        {
            _spawnPos = transform.position;
            _spawnRot = transform.rotation;
            _rigidbody =  GetComponent<Rigidbody>();
        }

        public void ResetFull()
        {
            if (_rigidbody)
            {
                _rigidbody.angularVelocity = Vector3.zero;
                _rigidbody.linearVelocity = Vector3.zero;
            }

            transform.position = _spawnPos;
            transform.rotation = _spawnRot;
        }
        
    }
}