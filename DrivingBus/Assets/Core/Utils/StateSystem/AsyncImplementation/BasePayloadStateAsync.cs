using System.Threading.Tasks;
using Core.Utils.StateSystem.AsyncImplementation.AsyncInterfaces;

namespace Core.Utils.StateSystem.AsyncImplementation
{
	public abstract class BasePayloadStateAsync<TPayload> : IPayloadStateAsync<TPayload>
	{
		public bool isActive { get; set; }
		public StateMachine StateMachine { get; set; }

		public abstract Task EnterAsync(TPayload payload);
		public abstract Task ExitAsync();
	}
}