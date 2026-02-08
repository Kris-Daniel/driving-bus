using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Core.Gameplay.Vehicles
{
    public class VehicleDestroyableChecker : MonoBehaviour
    {
        List<VehicleDestroyable> _vehicleDestroyables;

        void Awake()
        {
            _vehicleDestroyables = GetComponentsInChildren<VehicleDestroyable>().ToList();
        }

        void OnCollisionEnter(Collision other)
        {
            foreach (var vehicleDestroyable in _vehicleDestroyables)
            {
                vehicleDestroyable.CustomCollisionEnter(other);
            }
        }
    }
}