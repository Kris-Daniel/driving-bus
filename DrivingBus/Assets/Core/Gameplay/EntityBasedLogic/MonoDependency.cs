using UnityEngine;

namespace Core.Gameplay.EntityBasedLogic
{
    public abstract class MonoDependency : MonoBehaviour
    {
        protected ITimeInState TimeInState;
        protected ISetterForCharacterState SetterForCharacterState;

        public void SetupFromStateController(ITimeInState timeInState, ISetterForCharacterState setterForCharacterState)
        {
            SetterForCharacterState = setterForCharacterState;
            TimeInState = timeInState;
        }
        public abstract void Init();
        public abstract void Enter();
        public abstract void Exit();
    }
}