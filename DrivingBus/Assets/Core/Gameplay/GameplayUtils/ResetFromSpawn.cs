using Core.Boot.FlowInterfaces;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core.Gameplay.GameplayUtils
{
    public class ResetFromSpawn : MonoBehaviour, IResettable
    {
        Vector3 _spawnPos;
        Quaternion _spawnRot;
        
        Rigidbody _rigidbody;
        
        bool _isKinematic;
        
        void Awake()
        {
            _spawnPos = transform.position;
            _spawnRot = transform.rotation;
            _rigidbody =  GetComponent<Rigidbody>();
            if (_rigidbody)
            {
                _isKinematic =  _rigidbody.isKinematic;
            }
        }

        public async void ResetFull()
        {
            if (_rigidbody)
            {
                _rigidbody.angularVelocity = Vector3.zero;
                _rigidbody.linearVelocity = Vector3.zero;
                _rigidbody.isKinematic = true;
            }

            transform.position = _spawnPos;
            transform.rotation = _spawnRot;

            await UniTask.Yield();

            if (_rigidbody)
            {
                _rigidbody.isKinematic = _isKinematic;
            }
        }
        
    }
}