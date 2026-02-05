using System.Threading.Tasks;
using Core.Utils.StateSystem.Interfaces;

namespace Core.Utils.StateSystem.AsyncImplementation.AsyncInterfaces
{
	/// <summary>
	///     Exitable async state
	/// </summary>
	public interface IExitStateAsync : IBaseState
	{
		Task ExitAsync();
	}
}