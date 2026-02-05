using System.Threading.Tasks;

namespace Core.Utils.StateSystem.AsyncImplementation.AsyncInterfaces
{
	public interface IPayloadStateAsync<in TPayload> : IExitStateAsync
	{
		Task EnterAsync(TPayload payload);
	}
}