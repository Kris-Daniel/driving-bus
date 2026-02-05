using UnityEngine;

namespace Core.ScriptableData
{
    [CreateAssetMenu(fileName = "CharacterControlDataSO", menuName = "EntityData/CharacterControlDataSO")]
    public class CharacterControlDataSO : ScriptableObject
    {
        [Space][Header("Movement")]
        public float MaxSpeed = 10f;
        public float Acceleration;
        public float AccelerationInAir;
        public float MaxAccelerationForce = 100f;
        public float MaxAccelerationFactor;
        public float SpeedFactor = 1f;
        public AnimationCurve AccelerationFactorFromDot;
        public AnimationCurve MaxAccelerationForceFactorFromDot;
        public Vector3 ForceScale = Vector3.one;
        public float MovementInAirMultiplier = 0.1f;
        
        [Space][Header("Jump")]
        public float JumpForce = 10f;
        public float JumpMultiplierWhileMoving = 1.3f;
        public float JumpAngleWhileMoving = 45f;
		
        public float TimeOnGroundBeforeNewJump = 0.1f;
        public float JumpButtonBuffer = 0.2f;
        public float JumpOutsideBuffer = 0.2f;
        public float JumpForceByTimeMultiplier = 1f;
        public AnimationCurve JumpForceByTimeCurve;
        
        public float JumpGravityMultiplier = 1f;
        public AnimationCurve JumpGravityCurve;
        
        [Space] public float RotationSpeed = 180f;

        [Header("Suspension")] 
        public float BufferForSlopes;
        public float RayDistance;
        public float RideHeight;
        public float RideSpringStrength;
        public float RideSpringDamper;

        [Space][Header("Reset Rotation")]
        public float UprightJointSpringStrength;
        public float UprightJointSpringDamper;

        [Space][Header("Other")]
        public LayerMask CharacterLayer;
    }
}