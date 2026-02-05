using Core.Utils.StateSystem.Interfaces;

namespace Core.Utils.StateSystem
{
	public abstract class UpdatablePayloadState<TPayload> : BasePayloadState<TPayload>, IUpdateState
	{
		public abstract void Update();
		public abstract void FixedUpdate();
	}
}