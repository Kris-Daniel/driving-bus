using Core.Gameplay.EntityBasedLogic;
using Core.ScriptableData;
using UnityEngine;

namespace Core.Gameplay.Characters.CharacterControl
{
    public class CharacterJumpMD : MonoDependency
    {
        [SerializeField] CharacterControlDataSO _controlDataSO;
        
		[SerializeField] CharacterInputsMD _characterInputsMD;
		[SerializeField] CharacterGroundAirMD _characterGroundAirMD;

		Rigidbody _rigidbody;
		float _jumpTime = -1f;
		bool _isJumpAndNotFall;

		public bool IsJumpAndNotFall() => _isJumpAndNotFall;

		public override void Init()
		{
			_rigidbody = GetComponent<Rigidbody>();
		}

		public override void Enter() { }

		public override void Exit() { }

		void FixedUpdate()
		{
			HandleJump();
		}

		bool CanJump() => (StayedEnoughOnGroundForJump() || HasEnoughChaoticTimeForJump()) && TresholdForNewJump();

		bool StayedEnoughOnGroundForJump() => _characterGroundAirMD.GroundAndAirTimer > _controlDataSO.TimeOnGroundBeforeNewJump;

		bool HasEnoughChaoticTimeForJump() => _characterGroundAirMD.IsInAir() && (-_characterGroundAirMD.GroundAndAirTimer < _controlDataSO.JumpOutsideBuffer);

		bool TresholdForNewJump() => _jumpTime < 0;

		void HandleJump()
		{
			
			bool jumpPerformed = false;
			if (_characterInputsMD.IsJumpPressedAndBuffered() && CanJump())
			{
				PerformJump();
				jumpPerformed = true;
				_jumpTime = 0f;
			}
			
			if (_jumpTime > 0f || jumpPerformed)
			{
				_jumpTime += Time.fixedDeltaTime;
				
				if (_characterInputsMD.IsJumpPressedAndBuffered() && _rigidbody.linearVelocity.y > 0)
				{
					ApplyForceWhilePressingJumpInAir();
				}
			}

			if (_jumpTime > _controlDataSO.TimeOnGroundBeforeNewJump && (_rigidbody.linearVelocity.y <= 0 || _characterGroundAirMD.IsOnGround()))
			{
				EndJump();
			}

			if (_characterGroundAirMD.IsInAir())
			{
				ApplyForceWhileInAir();
			}
		}

		void ApplyForceWhileInAir()
		{
			_rigidbody.AddForce(Vector3.down * (_controlDataSO.JumpGravityMultiplier * _controlDataSO.JumpGravityCurve.Evaluate(_characterGroundAirMD.GroundAndAirTimer)), ForceMode.Acceleration);
		}

		void ApplyForceWhilePressingJumpInAir()
		{
			_rigidbody.AddForce(Vector3.up * (_controlDataSO.JumpForceByTimeMultiplier * _controlDataSO.JumpForceByTimeCurve.Evaluate(_jumpTime)), ForceMode.Acceleration);
		}

		void PerformJump()
		{
			_isJumpAndNotFall = true;
			
			var velocity = _rigidbody.linearVelocity;
			velocity.y = 0;
			//_rigidbody.linearVelocity = velocity;

			var upDir = Vector3.up;

			if (velocity.sqrMagnitude > 0.1f)
			{
				var velocityDir = velocity.normalized;
				
				// Rotate velocityDir around its right vector by 45 degree
				velocityDir = Quaternion.AngleAxis(_controlDataSO.JumpAngleWhileMoving, Vector3.Cross(velocityDir, Vector3.up)) * velocityDir;

				upDir = velocityDir * _controlDataSO.JumpMultiplierWhileMoving;
			}

			_rigidbody.AddForce(upDir * (_controlDataSO.JumpForce * _rigidbody.mass), ForceMode.Impulse);
		}

		void EndJump()
		{
			_isJumpAndNotFall = false;
			_jumpTime = -1f;
		}
    }
}