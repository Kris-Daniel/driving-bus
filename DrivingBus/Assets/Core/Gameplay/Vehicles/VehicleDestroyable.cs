using Core.Boot.FlowInterfaces;
using UnityEngine;

namespace Core.Gameplay.Vehicles
{
    public class VehicleDestroyable : MonoBehaviour, IResettable
    {
        [SerializeField] float _speedToBeDestroyed;
        [SerializeField] Collider _colliderToDetach;
        
        Transform _initialParent;
        Vector3 _initialPosition;
        Quaternion _initialRotation;
        Vector3 _initialScale;

        bool _isDetached;

        void Awake()
        {
            _initialParent = transform.parent;
            _initialPosition = transform.position;
            _initialRotation = transform.rotation;
            _initialScale = transform.localScale;
        }

        void OnCollisionEnter(Collision other)
        {
            if(_isDetached) return;
            _isDetached = true;
            
            var collisionImpulse = other.impulse.magnitude;

            if (collisionImpulse > _speedToBeDestroyed)
            {
                Detach();
            }
        }

        void Detach()
        {
            _colliderToDetach.transform.SetParent(null);
            _colliderToDetach.gameObject.AddComponent<Rigidbody>();
        }

        public void ResetFull()
        {
            if(!_isDetached) return;
            _isDetached = false;
            
            if (_colliderToDetach.gameObject.TryGetComponent(out Rigidbody rb))
            {
                Destroy(rb);
            }
            _colliderToDetach.transform.SetParent(_initialParent);
            _colliderToDetach.transform.localPosition = _initialPosition;
            _colliderToDetach.transform.localRotation = _initialRotation;
            _colliderToDetach.transform.localScale = _initialScale;
        }
    }
}