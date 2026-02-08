using Core.Utils.Observables;

namespace Core.Gameplay.Vehicles
{
    public class CarInputs
    {
        public ObservableField<bool> EngineIsOn =  new ObservableField<bool>(false);
        public float ForwardMoveAxis;
        public float ForwardMoveRaw;
        public float TurnAxis;
        public float TurnAxisRaw;
        public bool IsHandbrake;
        public bool IsNitro;

        public void ClearInputs()
        {
            EngineIsOn.Value = false;
            ForwardMoveAxis = 0;
            ForwardMoveRaw = 0;
            TurnAxis = 0;
            TurnAxisRaw = 0;
            IsHandbrake = false;
            IsNitro = false;
        }
    }
}