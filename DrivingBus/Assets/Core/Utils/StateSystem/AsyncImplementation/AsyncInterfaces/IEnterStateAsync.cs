using System.Threading.Tasks;
using Core.Utils.StateSystem.Interfaces;

namespace Core.Utils.StateSystem.AsyncImplementation.AsyncInterfaces
{
	/// <summary>
	///     Enterable async state
	/// </summary>
	public interface IEnterStateAsync : IBaseState
	{
		Task EnterAsync();
	}
}