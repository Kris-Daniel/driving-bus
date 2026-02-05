namespace Core.Utils.StateSystem.Interfaces
{
    /// <summary>
    ///     Base State interface
    /// </summary>
    public interface IBaseState
	{
		public bool isActive { get; set; }
		public StateMachine StateMachine { get; set; }
	}
}