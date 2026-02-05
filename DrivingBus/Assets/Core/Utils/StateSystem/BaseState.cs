using Core.Utils.StateSystem.Interfaces;

namespace Core.Utils.StateSystem
{
	public abstract class BaseState : IState
	{
		public bool isActive { get; set; }
		public StateMachine StateMachine { get; set; }

		public abstract void Enter();
		public abstract void Exit();
	}
}