using UnityEngine;

namespace Core.ScriptableData
{
    [CreateAssetMenu(fileName = "VehicleControlDataSO", menuName = "EntityData/VehicleControlDataSO")]
    public class VehicleControlDataSO : ScriptableObject
    {
        [Space] [Header("Engine")]
        public float CarTopSpeed = 60f;
        public AnimationCurve PowerCurve;
        public float AccelerationMultiplier = 15;
		
        [Space] [Header("Decelerate when moving by inertia")]
        public AnimationCurve BrakeCurve;
        public float BrakeCurveMultiplier = 0.01f;

        [Space] [Header("Global")] public float AdditionalGripFactor = 1;
		
        [Space] [Header("Hand Brake")]
        public float HandBrakeGrip = 0.05f;
        public float HandBrakeGripMultiplieForBackWheels = 0.7f;
        public AnimationCurve FullStopByHandBrakeCurve;
		
        [Space] [Header("Side Steering")]
        public float TireMass = 0.1f;
        public AnimationCurve SideSteeringGripBySpeedCurve;
        public float SideSteeringGripMultiplierForBackWheels = 0.7f;
        public float SideMultiplier = 10f;
		
		
        [Space] [Header("Suspension")]
        public float RayDistance = 0.8f;
        public float SuspensionStrength = 50f;
        public float Damping = 5f;
        public float SuspensionRestDist = 0.4f;
        public float WheelModelOffset = -0.2f;
        public float MaxWheelModelOffset = 0.1f;
        public float MinWheelModelOffset = -0.3f;

		
        [Space] [Header("Wheel Rotation")]
        public AnimationCurve WheelRotationLerpSpeedCurve;
        public float WheelRotationLerpSpeed;

        public AnimationCurve TurnAngleCurve;
        public float MinTurnAngle = 20f;
        public float MaxTurnAngle = 45f;

        [Space] [Header("Reverse Parameters")] 
        public float ReverseCarTopSpeed = 20f;
        public float ReverseAccelerationMultiplier = 10f;
        public float ReverseSideSteeringGripMultiplierForBackWheels = 1.5f;
        public float ReverseMinTurnAngle = 45f;

        [Space] [Header("Air Parameters")] 
        public float AirForceMultiplier = 0.1f;
        public AnimationCurve AirDampingCurve;
        public float AirDamping;
        public float AirAdditionalGravity = -1f;
    }
}