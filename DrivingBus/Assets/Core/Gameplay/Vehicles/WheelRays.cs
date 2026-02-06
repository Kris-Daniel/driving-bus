using Core.ScriptableData;
using UnityEngine;

namespace Core.Gameplay.Vehicles
{
    public class WheelRays : MonoBehaviour
    {
        [SerializeField] LayerMask _carLayer;
        [SerializeField] bool _isLeftWheel;
        [SerializeField] VehicleControlDataSO _vehicleControlData;
		
        void Update()
        {
            Debug.DrawRay(transform.position, _isLeftWheel ? -transform.right : transform.right, Color.yellow);

            if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, _vehicleControlData.RayDistance, ~_carLayer))
            {
                Debug.DrawRay(transform.position, -transform.up * hit.distance, Color.red);
            }
            else
            {
                Debug.DrawRay(transform.position, -transform.up * 0.45f, Color.green);
            }
        }
    }
}