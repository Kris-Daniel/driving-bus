namespace Core.Utils.StateSystem.Interfaces
{
    /// <summary>
    ///     Exitable state
    /// </summary>
    public interface IExitState : IBaseState
	{
		void Exit();
	}
}