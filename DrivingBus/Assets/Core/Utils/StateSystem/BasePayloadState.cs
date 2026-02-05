using Core.Utils.StateSystem.Interfaces;

namespace Core.Utils.StateSystem
{
	public abstract class BasePayloadState<TPayload> : IPayloadState<TPayload>
	{
		public bool isActive { get; set; }
		public StateMachine StateMachine { get; set; }

		public abstract void Enter(TPayload payload);
		public abstract void Exit();
	}
}