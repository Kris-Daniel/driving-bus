using System.Collections.Generic;
using System.Linq;
using Core.Boot.FlowInterfaces;
using Core.ScriptableData;
using UnityEngine;

namespace Core.Gameplay.Vehicles
{
    public class CarParameters : MonoBehaviour, IResettable
	{
		[SerializeField] VehicleControlDataSO _vehicleControlDataSo;
		
		public VehicleControlDataSO Data => _vehicleControlDataSo;
		public Rigidbody Rigidbody { get; private set; }

		float _normalizedSpeedDebug;
		bool _isReverseMovementDebug;
		List<FullWheelControl> _wheels = new List<FullWheelControl>();
		public List<FullWheelControl> Wheels => _wheels;

		CarInputs _carInputs;

		public bool IsHandBrake { get; private set; }
		public bool IsNitro { get; private set; }
		public float TurnAxis { get; private set; }
		public float TurnAxisRaw { get; private set; }
		public float ForwardMoveRaw { get; private set; }
		public float ForwardMoveAxis { get; private set; }

		public float DistancePerFrame { get; private set; }
		
		Vector3 _previousPosition;
		
		public float GroundAndAirTimer { get; private set; }
		
		public float LastGroundHitForce { get; private set; }
		public bool IsOverWater { get; private set; }
		
		void Awake()
		{
			Rigidbody = GetComponent<Rigidbody>();
			_wheels = GetComponentsInChildren<FullWheelControl>().ToList();
			_carInputs = GetComponent<VehicleBase>().CarInputs;
			
			_previousPosition = Rigidbody.position;
		}

		void Update()
		{
			if(_carInputs == null) return;
			
			ForwardMoveAxis = _carInputs.ForwardMoveAxis;
			ForwardMoveRaw = _carInputs.ForwardMoveRaw;
			TurnAxis = _carInputs.TurnAxis;
			TurnAxisRaw = _carInputs.TurnAxisRaw;
			IsHandBrake = _carInputs.IsHandbrake;
			IsNitro = _carInputs.IsNitro;
		}

		void FixedUpdate()
		{
			_normalizedSpeedDebug = NormalizedSpeed();
			_isReverseMovementDebug = IsReverseMovement();
			
			DistancePerFrame = Vector3.Distance(Rigidbody.position, _previousPosition);
			
			_previousPosition = Rigidbody.position;
			
			if (!IsInAir())
			{
				IncrementGroundTimer();
			}
			else
			{
				IncrementAirTimer();
			}
			
			LastGroundHitForce = Mathf.Lerp(LastGroundHitForce, 0, Time.fixedDeltaTime * 2);
		}

		public float NormalizedSpeed()
		{
			float carSpeed = Vector3.Dot(Rigidbody.transform.forward, Rigidbody.linearVelocity);
			return Mathf.Clamp01(Mathf.Abs(carSpeed) / Data.CarTopSpeed);
		}
		
		public bool IsReverseMovement()
		{
			var forwardMovement = Vector3.Dot(Rigidbody.transform.forward, Rigidbody.linearVelocity.normalized);
			return forwardMovement < 0;
		}

		public bool IsDrift()
		{
			foreach (var fullWheelControl in _wheels)
			{
				if (fullWheelControl.IsDrift)
				{
					return true;
				}
			}

			return false;
		}
		
		public bool IsInAir()
		{
			foreach (var fullWheelControl in _wheels)
			{
				if (!fullWheelControl.IsInAir)
				{
					return false;
				}
			}

			return true;
		}
		
		public Vector3 CenterOfWheels()
		{
			Vector3 center = Vector3.zero;
			foreach (var fullWheelControl in _wheels)
			{
				center += fullWheelControl.transform.position;
			}

			return center / _wheels.Count;
		}
		
		void IncrementAirTimer()
		{
			if (GroundAndAirTimer > 0)
			{
				GroundAndAirTimer = -Time.fixedDeltaTime;
			}
			else
			{
				GroundAndAirTimer -= Time.fixedDeltaTime;
			}
		}


		void IncrementGroundTimer()
		{
			if (GroundAndAirTimer < 0)
			{
				LastGroundHitForce = Mathf.Abs(Rigidbody.linearVelocity.y);
				GroundAndAirTimer = Time.fixedDeltaTime;
			}
			else
			{
				GroundAndAirTimer += Time.fixedDeltaTime;
			}
		}

		public void ResetFull()
		{
			LastGroundHitForce = 0;
		}
    }
}