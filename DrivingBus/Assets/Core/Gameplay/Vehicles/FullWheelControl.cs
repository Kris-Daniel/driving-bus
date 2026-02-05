using Core.Boot.FlowInterfaces;
using Core.ScriptableData;
using Core.Utils.Extensions;
using UnityEngine;

namespace Core.Gameplay.Vehicles
{
    public class FullWheelControl : MonoBehaviour, IResettable
	{
		[SerializeField] CarParameters _carParametersObject;
		[SerializeField] Transform _wheelModel;
		[SerializeField] bool _canAccelerate;
		[SerializeField] bool _canTurn;
		[SerializeField] bool _canBrake;
		[SerializeField] bool _isLeftTire;
		[SerializeField] bool _isBackTire;
		[SerializeField] LayerMask _carLayer;
		
		float _forwardMoveAxis;
		float _forwardMoveRaw;
		bool _isHandBrake;
		float _turnAxis;
		bool _isReverseMove;
		public bool IsDrift { get; private set; }
		public bool IsInAir { get; private set; }
		public RaycastHit GroundHit { get; private set; }
		
		VehicleControlDataSO _carParameters;

		float _suspensionStrengthToUse;
		float _suspensionDampingToUse;

		void Awake()
		{
			if (_carParameters == null)
			{
				_carParametersObject = GetComponentInParent<CarParameters>();
			}
			
			_carParameters = _carParametersObject.Data;
			
			_suspensionStrengthToUse = _carParameters.SuspensionStrength;
			_suspensionDampingToUse = _carParameters.Damping;
		}

		void Update()
		{
			_forwardMoveAxis = _carParametersObject.ForwardMoveAxis;
			_forwardMoveRaw = _carParametersObject.ForwardMoveRaw;
			_turnAxis = _carParametersObject.TurnAxis;

			if (_forwardMoveRaw < 0 && _carParametersObject.GroundAndAirTimer >= 0 && _carParametersObject.GroundAndAirTimer < 0.35f)
			{
				_isHandBrake = true;
			}
			else
			{
				_isHandBrake = _carParametersObject.IsHandBrake;
			}
		}

		void FixedUpdate()
		{
			if (_carParametersObject == null)
			{
				GroundHit = default;
				return;
			}

			CalculateSuspensionsForce();
			
			_isReverseMove = _carParametersObject.IsReverseMovement();
			
			WheelTurn();

			if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, _carParameters.RayDistance, ~_carLayer))
			{
				GroundHit = hit;
				
				WheelMove(hit);
				
				IsInAir = false;
			}
			else
			{
				GroundHit = default;
				float restDist = (_carParameters.SuspensionRestDist - _carParameters.RayDistance);
				restDist = Mathf.Clamp(restDist, _carParameters.MinWheelModelOffset, _carParameters.MaxWheelModelOffset);
				var carParametersSuspensionRestDist = Vector3.up * restDist;
				_wheelModel.localPosition = Vector3.Lerp(_wheelModel.localPosition, carParametersSuspensionRestDist, Time.deltaTime * 10f);
				IsInAir = true;
				if (_carParametersObject.IsInAir()/* && _carParametersObject.GroundAndAirTimer > -2.1f*/)
				{
					if (_carParametersObject.IsOverWater)
					{
						CalculateForceToRecenterCarInAir();
					}
				}
				StopEmitTrail();
			}
			
			WhellModelRotation();
		}

		void CalculateSuspensionsForce()
		{
			float suspensionStrength = _carParametersObject.GroundAndAirTimer >= 0
				? _carParameters.SuspensionStrength
				: _carParameters.SuspensionStrength * 3f;
			
			float suspensionDamping = _carParametersObject.GroundAndAirTimer >= 0
				? _carParameters.Damping
				: _carParameters.Damping * 3f;
			
			_suspensionStrengthToUse = Mathf.Lerp(_suspensionStrengthToUse, suspensionStrength, Time.fixedDeltaTime * 5f);
			_suspensionDampingToUse = Mathf.Lerp(_suspensionDampingToUse, suspensionDamping, Time.fixedDeltaTime * 5f);
		}

		void CalculateForceToRecenterCarInAir()
		{
			var carUp = _carParametersObject.Rigidbody.transform.up;
			bool isTurnedAround = Vector3.Dot(carUp, Vector3.up) < 0;
			
			if(isTurnedAround) return;
			
			var centerOfWheels = _carParametersObject.CenterOfWheels();
			var transformPosition = transform.position;

			if (transformPosition.y > centerOfWheels.y)
			{
				return;
			}
			
			var upDirectionToRecenter = centerOfWheels - transformPosition;

			upDirectionToRecenter.x = 0;
			upDirectionToRecenter.z = 0;

			float offset = upDirectionToRecenter.magnitude * (centerOfWheels.y > transformPosition.y ? 1 : 0);

			if (offset > 0)
			{
				// Calculate the vertical velocity component
				float distanceToCenterOfWheel = (centerOfWheels - transformPosition).magnitude;
				
				float verticalVelocity = Vector3.Dot(_carParametersObject.Rigidbody.GetPointVelocity(transform.position), Vector3.up);
				
				float force = (offset * _carParameters.AirForceMultiplier) - (verticalVelocity * _carParameters.AirDamping);
				force *= _carParameters.AirDampingCurve.Evaluate(Mathf.Abs(offset) / distanceToCenterOfWheel);
				
				_carParametersObject.Rigidbody.AddForceAtPosition(Vector3.up * force, transformPosition, ForceMode.Acceleration);
			}
			
			_carParametersObject.Rigidbody.AddForceAtPosition(Vector3.up * _carParameters.AirAdditionalGravity,_carParametersObject.Rigidbody.transform.position, ForceMode.Acceleration);
		}

		void StopEmitTrail()
		{
			IsDrift = false;
		}

		void WheelMove(RaycastHit hit)
		{
		    var tireWorldVel = _carParametersObject.Rigidbody.GetPointVelocity(transform.position);
		    var finalForce = Vector3.zero;

		    // Get the car's speed in m/s
		    float currentSpeed = _carParametersObject.Rigidbody.linearVelocity.magnitude;

		    if (IsAccelerating())
		    {
		        var accelerationForce = AccelerationForce();
		        finalForce += accelerationForce;
		    }
		    
		    // Refactored inertia deceleration with improved grip
		    if (_forwardMoveRaw == 0 && !_isHandBrake)
		    {
			    var brakeForce = BrakeForce(tireWorldVel, _carParameters.BrakeCurve.Evaluate(_carParametersObject.NormalizedSpeed()) * _carParameters.BrakeCurveMultiplier);
			    finalForce += brakeForce;
			    
			    Vector3 velocityDirection = _carParametersObject.Rigidbody.linearVelocity.normalized;
			    float speedFactor = _carParameters.BrakeCurve.Evaluate(_carParametersObject.NormalizedSpeed());
            
			    // Calculate grip based on speed and surface
			    float dynamicGrip = CalculateDynamicGrip(tireWorldVel, hit);
			    Vector3 decelerationForce = -velocityDirection * (speedFactor * _carParameters.BrakeCurveMultiplier * dynamicGrip);
            
			    finalForce += decelerationForce;
		    }

		    var sideSteeringForce = SideSteeringForceSimple(tireWorldVel, _carParameters.SideSteeringGripBySpeedCurve.Evaluate(_carParametersObject.NormalizedSpeed()));
		    
		    var currentGrip = sideSteeringForce;

		    if (_isHandBrake && _canBrake)
		    {
		        var handBrakeForce = HandBrakeForce(tireWorldVel);
		        currentGrip += handBrakeForce;
		    }

		    currentGrip *= _carParameters.AdditionalGripFactor;

		    if (!_isHandBrake)
		    {
			    float gripAfterLanding = 1f;

			    if (_carParametersObject.GroundAndAirTimer > 0)
			    {
				    gripAfterLanding = _carParametersObject.GroundAndAirTimer.RemapClamped(0f, 1f, 0.3f, 1f);
			    }
				currentGrip *= gripAfterLanding;
		    }
		    
		    finalForce += currentGrip;
		    
		    var suspensionForce = SuspensionForce(tireWorldVel, hit.distance);
		    finalForce += suspensionForce;
		    
		    EmitTireTrail(tireWorldVel);
		    
		    _carParametersObject.Rigidbody.AddForceAtPosition(finalForce, transform.position, ForceMode.Acceleration);
		}
		
		float CalculateDynamicGrip(Vector3 tireVelocity, RaycastHit hit)
		{
			float baseGrip = 1.0f;
    
			float speed = tireVelocity.magnitude;
			float speedGripFactor = Mathf.Clamp01(1.0f - (speed / _carParameters.CarTopSpeed));
    
			// Surface-based grip modification (assuming hit has surface info)
			float surfaceGrip = 1.0f; // Default value
			return baseGrip * speedGripFactor * surfaceGrip * _carParameters.AdditionalGripFactor;
		}

		void EmitTireTrail(Vector3 tireWorldVel)
		{
			var steeringDir = transform.right;
			float steeringVel = Vector3.Dot(tireWorldVel, steeringDir);

			float percentOfTireVelocityInRightDirection =  Mathf.Abs(steeringVel) / tireWorldVel.magnitude;
			
			IsDrift = (percentOfTireVelocityInRightDirection > 0.35f || _isHandBrake) && _carParametersObject.Rigidbody.GetPointVelocity(transform.position).sqrMagnitude > 3f;
		}

		bool IsAccelerating()
		{
			return ((_canAccelerate && !_canBrake) || (_canAccelerate && _canBrake && !_isHandBrake)) && Mathf.Abs(_forwardMoveAxis) > 0.001f;
		}

		Vector3 HandBrakeForce(Vector3 tireWorldVel)
		{
			var handBrakeForce = Vector3.zero;
			float linearVelocityMagnitude = _carParametersObject.Rigidbody.linearVelocity.magnitude;
			
			var percent = 1f - Mathf.Clamp01(linearVelocityMagnitude / _carParameters.CarTopSpeed);
			percent = 0.25f + percent * 0.75f;
			handBrakeForce += BrakeForce(tireWorldVel, percent * _carParameters.HandBrakeGrip) * (_isBackTire ? _carParameters.HandBrakeGripMultiplieForBackWheels : 1f);

			if (linearVelocityMagnitude < 5)
			{
				handBrakeForce += BrakeForce(tireWorldVel, _carParameters.FullStopByHandBrakeCurve.Evaluate(linearVelocityMagnitude / 5f));
				handBrakeForce += SideSteeringForceSimple(tireWorldVel, _carParameters.FullStopByHandBrakeCurve.Evaluate(linearVelocityMagnitude / 5f));
			}

			return handBrakeForce;
		}

		void WhellModelRotation()
		{
			Vector3 tireWorldVel;
			// Calculate and apply wheel rotation
			tireWorldVel = _carParametersObject.Rigidbody.GetPointVelocity(transform.position);
			if (_carParametersObject.IsInAir())
			{
				tireWorldVel = Vector3.zero;
			}
			float diameter = 1.2f;
			float wheelCircumference = 2 * Mathf.PI * diameter / 2;
			float distanceTraveled = tireWorldVel.magnitude * Time.fixedDeltaTime;
			float rotationAngle = (distanceTraveled / wheelCircumference) * 360f;
			
			float velocityDirection = Vector3.Dot(tireWorldVel.normalized, _carParametersObject.Rigidbody.transform.forward);

			// Adjust rotation based on acceleration direction
			if (velocityDirection > 0)
			{
				rotationAngle = -rotationAngle;
			}

			if (!_isLeftTire)
			{
				// rotationAngle = -rotationAngle;
			}
			
			rotationAngle = -rotationAngle;

			// Apply braking logic to affect wheel rotation
			if (_isHandBrake && _canBrake)
			{
				rotationAngle *= _carParameters.HandBrakeGrip; // Reduce rotation speed when braking
			}

			_wheelModel.localRotation *= Quaternion.Euler(rotationAngle, 0f, 0f);
		}
		
		

		void WheelTurn()
		{
			if(!_canTurn) return;
			
			var turnRotation = TurnRotation(_turnAxis);
			
			var coef = 1f;
			if (Mathf.Abs(_turnAxis) < 0.1f)
			{
				coef = 3f;
			}
			else
			{
				var localRotationFromForward = Quaternion.Euler(0, transform.localEulerAngles.y, 0) * Vector3.forward;
				var direction = Vector3.SignedAngle(Vector3.forward, localRotationFromForward, Vector3.up);

				if (direction < 0 && _turnAxis > 0 || direction > 0 && _turnAxis < 0)
				{
					coef = 3f;
				}
				
			}
			
			var lerpMultiplier = _carParameters.WheelRotationLerpSpeed * coef * Time.deltaTime;
			
			transform.localRotation = Quaternion.Lerp(transform.localRotation, turnRotation, lerpMultiplier);
		}
		
		Vector3 SideSteeringForceSimple(Vector3 tireWorldVel, float grip)
		{
			float multiplierByGroundTime = Mathf.Clamp(_carParametersObject.GroundAndAirTimer, 0, 1f);
			
			var steeringDir = transform.right;
			
			float tireGripFactor = grip;

			if (_isBackTire)
			{
				tireGripFactor *= _isReverseMove ? _carParameters.ReverseSideSteeringGripMultiplierForBackWheels : _carParameters.SideSteeringGripMultiplierForBackWheels;
			}
			
			float desiredVelChange = -Vector3.Dot(steeringDir, tireWorldVel) * tireGripFactor;
			float desiredAccel = desiredVelChange / Time.fixedDeltaTime;
				
			return steeringDir * (_carParameters.TireMass * desiredAccel * multiplierByGroundTime);
		}

		Vector3 SuspensionForce(Vector3 tireWorldVel, float hitDistance)
		{
			var springDir = transform.up;
			
			float offset = _carParameters.SuspensionRestDist - hitDistance;
			
			if (hitDistance > _carParameters.RayDistance)
			{
				return springDir * (-9f);
			}

			var modelOffset = offset + _carParameters.WheelModelOffset;
			modelOffset = Mathf.Clamp(modelOffset, _carParameters.MinWheelModelOffset, _carParameters.MaxWheelModelOffset);
			_wheelModel.localPosition = Vector3.up * modelOffset;
				
			float velocity = Vector3.Dot(springDir, tireWorldVel);
			
			float force = (offset * _suspensionStrengthToUse) - (velocity * _suspensionDampingToUse);

			return springDir * force;
		}


		Vector3 BrakeForce(Vector3 tireWorldVel, float brakeMultiplier)
		{
			var forwardDir = _carParametersObject.Rigidbody.transform.forward;
			var steeringVel = Vector3.Dot(forwardDir, tireWorldVel);
			var desiredVelChange = -steeringVel * brakeMultiplier;
			var desiredAccel = desiredVelChange / Time.fixedDeltaTime;

			var force = forwardDir * (_carParameters.TireMass * desiredAccel);
			return force;
		}

		Vector3 AccelerationForce()
		{
			Vector3 accelDir = transform.forward;
			
			var carTopSpeed = _isReverseMove ? _carParameters.ReverseCarTopSpeed : _carParameters.CarTopSpeed;
			var carAccelerationMultiplier = _isReverseMove ? _carParameters.ReverseAccelerationMultiplier : _carParameters.AccelerationMultiplier;
			
			float carSpeed = Vector3.Dot(_carParametersObject.Rigidbody.transform.forward, _carParametersObject.Rigidbody.linearVelocity);
			float normalizedSpeed = Mathf.Clamp01(Mathf.Abs(carSpeed) / carTopSpeed);
			float availableTorque = _carParameters.PowerCurve.Evaluate(normalizedSpeed) * _forwardMoveAxis * _forwardMoveRaw * carAccelerationMultiplier;
			var force = accelDir * availableTorque;

			return force;
		}

		Quaternion TurnRotation(float turnAxis)
		{
			return Quaternion.Euler(0, ConvertRange(turnAxis), 0);
		}
		
		float ConvertRange(float value)
		{
			float normalizedValue = Mathf.InverseLerp(-1f, 1f, value);
			bool isReverseMove = _carParametersObject.IsReverseMovement();
			
			var minAngle = isReverseMove ? _carParameters.ReverseMinTurnAngle : _carParameters.MinTurnAngle;

			float angle = Mathf.Lerp(minAngle, _carParameters.MaxTurnAngle, _carParameters.TurnAngleCurve.Evaluate(_carParametersObject.NormalizedSpeed()));
			
			float convertedValue = Mathf.Lerp(-angle, angle, normalizedValue);
			return convertedValue;
		}


		public void ResetFull()
		{
			transform.localRotation = Quaternion.identity;
		}

		public float GetNormalizedTurnRotation()
		{
			float maxTurnedAngle = _carParameters.MaxTurnAngle;

			var direction = transform.localRotation * Vector3.forward;
			
			float signedAngle = Vector3.SignedAngle(Vector3.forward, direction, Vector3.up);
			
			float turnAngle = signedAngle.RemapClamped(-maxTurnedAngle, maxTurnedAngle, 0f, 1f);

			
			return turnAngle;
		}
	}
}