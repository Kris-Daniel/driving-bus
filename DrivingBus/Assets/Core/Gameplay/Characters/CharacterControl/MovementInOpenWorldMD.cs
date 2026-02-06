using Core.Gameplay.EntityBasedLogic;
using Core.ScriptableData;
using UnityEngine;

namespace Core.Gameplay.Characters.CharacterControl
{
    public class MovementInOpenWorldMD : MonoDependency
    {
        [SerializeField] CharacterControlDataSO _controlDataSO;
		[SerializeField] CharacterInputsMD _playerMovementInputsMD;
		[SerializeField] CharacterGroundAirMD _playerGroundAirMD;
		[SerializeField] CharacterJumpMD _playerJumpMD;

		Rigidbody _rigidbody;
		Quaternion _lookDirection = Quaternion.identity;
		Vector3 _goalVel;

		float _jumpTime;

		public override void Init()
		{
			_rigidbody = GetComponent<Rigidbody>();
		}

		public override void Enter()
		{
			enabled = true;
		}

		public override void Exit()
		{
			enabled = false;
		}
		
		public float Speed() => _rigidbody.linearVelocity.magnitude;

		void FixedUpdate()
        {
            Vector3 groundVel = Vector3.zero;
            
            if (_playerGroundAirMD.IsOnGroundForSlopes() && !_playerJumpMD.IsJumpAndNotFall())
            {
	            Suspension(_playerGroundAirMD.GroundHit);

                groundVel = CalculateGroundVelocity(_playerGroundAirMD.GroundHit.rigidbody);
                
            }
            
			ApplyMovementForce(groundVel);
            
            // RotateAround();

            UpdateUprightForce();
            
            DebugRays();
        }

        void RotateAround()
        {
	        var moveDirection = _playerMovementInputsMD.MoveDirection();
	        if (moveDirection.sqrMagnitude > 0.01f)
	        {
		        Vector3 moveDir = moveDirection;

		        if (moveDir.sqrMagnitude > 0.01f)
		        {
			        _lookDirection = Quaternion.LookRotation(moveDir);
		        }

		        float maxDegreesDelta = _controlDataSO.RotationSpeed * Time.fixedDeltaTime;

		        transform.rotation = Quaternion.RotateTowards(transform.rotation, _lookDirection, maxDegreesDelta);
	        }
        }
        
        void ApplyMovementForce(Vector3 groundVel)
        {
            // Dot product between the movement direction and velocity direction for acceleration scaling
            float velDot = Vector3.Dot(_playerMovementInputsMD.MoveDirection().normalized, _rigidbody.linearVelocity.normalized);
            var acceleration = _playerGroundAirMD.IsInAir() ? _controlDataSO.AccelerationInAir : _controlDataSO.Acceleration;
            float accelFactor = acceleration * _controlDataSO.AccelerationFactorFromDot.Evaluate(velDot);

            // Calculate desired goal velocity, adding ground velocity
            Vector3 goalVelocity = (_playerMovementInputsMD.MoveDirection() * (_controlDataSO.MaxSpeed * _controlDataSO.SpeedFactor)) + groundVel;
            _goalVel = Vector3.MoveTowards(_goalVel, goalVelocity, accelFactor * Time.fixedDeltaTime);

            // Calculate needed acceleration based on time step
            Vector3 neededAccel = (_goalVel - _rigidbody.linearVelocity) / Time.fixedDeltaTime;

            // Clamp the needed acceleration to max force based on evaluation curve
            float maxAccelForce = _controlDataSO.MaxAccelerationForce * _controlDataSO.MaxAccelerationForceFactorFromDot.Evaluate(velDot) * _controlDataSO.MaxAccelerationFactor;
            neededAccel = Vector3.ClampMagnitude(neededAccel, maxAccelForce);

            var movementInAirMultiplier = _playerGroundAirMD.IsInAir() ? _controlDataSO.MovementInAirMultiplier : 1f;

            // Apply scaled force based on mass
            _rigidbody.AddForce(Vector3.Scale(neededAccel * _rigidbody.mass, _controlDataSO.ForceScale) * movementInAirMultiplier);
        }

        Vector3 CalculateGroundVelocity(Rigidbody hitRigidbody)
        {
	        Vector3 groundVel = Vector3.zero;
	        
	        if (hitRigidbody != null)
	        {
		        if (hitRigidbody.isKinematic)
		        {
			        /*if (hitRigidbody.TryGetComponent<KinematicVelocity>(out var kinematicVelocity))
			        {
				        groundVel = kinematicVelocity.LinearSpeed;
			        }*/
		        }
		        else
		        {
			        groundVel = hitRigidbody.linearVelocity;
		        }
	        }

	        return groundVel;
        }

        void Suspension(RaycastHit hit)
        {
            Vector3 vel = _rigidbody.linearVelocity;
            Vector3 rayDir = Vector3.down;

            Vector3 otherVel = Vector3.zero;

            Rigidbody hitBody = hit.rigidbody;
            if (hitBody != null)
            {
                otherVel = hitBody.linearVelocity;
            }

            float rayDirVel = Vector3.Dot(rayDir, vel);
            float otherDirVel = Vector3.Dot(rayDir, otherVel);

            float relVel = rayDirVel - otherDirVel;
            float x = hit.distance - _controlDataSO.RideHeight + _controlDataSO.BufferForSlopes;

            float springForce = (x * _controlDataSO.RideSpringStrength * _rigidbody.mass) - (relVel * _controlDataSO.RideSpringDamper * _rigidbody.mass);

            _rigidbody.AddForce(rayDir * springForce);

            if (hitBody != null)
            {
                hitBody.AddForceAtPosition(rayDir * -springForce, hit.point);
            }
        }

        void UpdateUprightForce()
        {
            Quaternion rotationDelta = ShortestRotation(transform.rotation, _lookDirection);
            
            Vector3 torque = CalculateSpringDamperTorque(rotationDelta, _controlDataSO.UprightJointSpringStrength, _controlDataSO.UprightJointSpringDamper);

            _rigidbody.AddTorque(torque);
        }

        Quaternion ShortestRotation(Quaternion from, Quaternion to)
        {
            if (Quaternion.Dot(from, to) < 0.0f)
                to = new Quaternion(-to.x, -to.y, -to.z, -to.w);

            return Quaternion.Inverse(from) * to;
        }

        Vector3 CalculateSpringDamperTorque(Quaternion rotationDelta, float spring, float damper)
        {
            rotationDelta.ToAngleAxis(out float angle, out Vector3 axis);
            if (angle > 180f) angle -= 360f;
            Vector3 torque = axis * (angle * Mathf.Deg2Rad * spring);

            torque -= _rigidbody.angularVelocity * damper;

            return torque;
        }
        void DebugRays()
        {
            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, _controlDataSO.RayDistance, ~_controlDataSO.CharacterLayer))
            {
                Debug.DrawRay(transform.position, Vector3.down * (hit.distance + _controlDataSO.BufferForSlopes), Color.green);
            }
            else
            {
                Debug.DrawRay(transform.position, Vector3.down * (_controlDataSO.RayDistance + _controlDataSO.BufferForSlopes), Color.red);
            }
        }
    }
}