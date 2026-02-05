using Core.Utils.StateSystem.Interfaces;

namespace Core.Utils.StateSystem
{
	public abstract class UpdatableState : BaseState, IUpdateState
	{
		public abstract void Update();
		public abstract void FixedUpdate();
	}
}