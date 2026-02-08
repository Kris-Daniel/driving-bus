using System;
using Core.Boot.FlowInterfaces;
using UnityEngine;

namespace Core.Gameplay.Vehicles
{
    public class VehicleDestroyable : MonoBehaviour, IResettable
    {
        [SerializeField] float _speedToBeDestroyed;
        [SerializeField] float _impulseToBeDestroyed = 2000; 
        [SerializeField] Collider _colliderToDetach;
        
        Transform _initialParent;
        Vector3 _initialPosition;
        Quaternion _initialRotation;
        Vector3 _initialScale;

        bool _isDetached;
        
        Rigidbody _attachedRigidbody;

        void Awake()
        {
            _initialParent = transform.parent;
            _initialPosition = transform.localPosition;
            _initialRotation = transform.localRotation;
            _initialScale = transform.localScale;
            _colliderToDetach.enabled = false;

            _attachedRigidbody = GetComponentInParent<Rigidbody>();
        }

        public void CustomCollisionEnter(Collision other)
        {
            if(_isDetached) return;
            
            var collisionImpulse = other.impulse.magnitude;
            
            Debug.Log("Collision impulse: " + collisionImpulse);

            if (collisionImpulse > _impulseToBeDestroyed)
            {
                var speed = _attachedRigidbody.linearVelocity.magnitude;
                Debug.Log("Speed: " + speed);

                if (speed > _speedToBeDestroyed)
                {
                    Detach();
                }
            }
        }

        void Detach()
        {
            _isDetached = true;
            _colliderToDetach.transform.SetParent(null);
            _colliderToDetach.gameObject.AddComponent<Rigidbody>();
            _colliderToDetach.enabled = true;
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
            _colliderToDetach.enabled = false;
        }
    }
}