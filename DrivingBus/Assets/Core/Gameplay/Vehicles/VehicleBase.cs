using System;
using Core.Gameplay.EntityBasedLogic;
using Core.Gameplay.Vehicles.VehicleMonoDependencies;
using UnityEngine;

namespace Core.Gameplay.Vehicles
{
    public class VehicleBase : MonoBehaviour
    {
        [SerializeField] MonoDependencyStateController _stateController;
        [SerializeField] VehicleControlByPlayerInputMD _controlByPlayerInputMD;
        [SerializeField] VehicleNoControlMD _vehicleNoControlMD;
        [SerializeField] VehicleSoundsMD _vehicleSoundsMD;
        
        public Transform DriverPosition;
        public Transform ExitCarPosition;
        public CarInputs CarInputs { get; private set; } = new CarInputs();

        void Awake()
        {
            _stateController.AddStateMD(VehicleStateConstants.DriveByPlayer, _controlByPlayerInputMD);
            _stateController.AddStateMD(VehicleStateConstants.NoDriving, _vehicleNoControlMD);
            
            _stateController.AddStateMD(VehicleStateConstants.DriveByPlayer, _vehicleSoundsMD);
            _stateController.AddStateMD(VehicleStateConstants.NoDriving, _vehicleSoundsMD);
            
            _stateController.SetState(VehicleStateConstants.NoDriving);
        }

        public void DriveByPlayer()
        {
            _stateController.SetState(VehicleStateConstants.DriveByPlayer);
        }

        public void StopDriving()
        {
            _stateController.SetState(VehicleStateConstants.NoDriving);
        }
    }
}