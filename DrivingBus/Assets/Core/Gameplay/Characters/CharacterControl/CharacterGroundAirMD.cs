using Core.Boot.FlowInterfaces;
using Core.Gameplay.EntityBasedLogic;
using Core.ScriptableData;
using UnityEngine;

namespace Core.Gameplay.Characters.CommonMD
{
    public class CharacterGroundAirMD : MonoDependency, IResettable
    {
        [SerializeField] CharacterControlDataSO _controlDataSO;

		RaycastHit _groundHit;
		bool _isOnGroundForSlopes;
		public RaycastHit GroundHit => _groundHit;
		
		public float GroundAndAirTimer { get; private set; }
		
		public bool IsInAir() => GroundAndAirTimer < 0;
		
		public bool IsOnGround() => GroundAndAirTimer > 0;

		public bool IsOnGroundForSlopes() => _isOnGroundForSlopes;

		Rigidbody _rigidbody;

		public override void Init()
		{
			_rigidbody = GetComponent<Rigidbody>();
		}

		public override void Enter()
		{
			
		}

		public override void Exit()
		{
			
		}

		void FixedUpdate()
		{
			if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, _controlDataSO.RideHeight, ~_controlDataSO.CharacterLayer))
			{
				_isOnGroundForSlopes = true;
			}
			else
			{
				_isOnGroundForSlopes = false;
			}
			
			if (Physics.Raycast(transform.position, Vector3.down, out _groundHit, _controlDataSO.RayDistance, ~_controlDataSO.CharacterLayer))
			{
				IncrementGroundTimer();
			}
			else
			{
				_groundHit = default;
				IncrementAirTimer();
			}
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
				GroundAndAirTimer = Time.fixedDeltaTime;
			}
			else
			{
				GroundAndAirTimer += Time.fixedDeltaTime;
			}
		}

		public void ResetFull()
		{
			if (!_rigidbody.isKinematic)
			{
				_rigidbody.linearVelocity = Vector3.zero;
			}
		}
    }
}