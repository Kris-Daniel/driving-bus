using System.Threading.Tasks;
using Core.Utils.StateSystem.AsyncImplementation.AsyncInterfaces;
using Core.Utils.StateSystem.Interfaces;

namespace Core.Utils.StateSystem.AsyncImplementation
{
	public class StateMachineAsync : StateMachine
	{
		public virtual async Task EnterAsync<TState>() where TState : class, IStateAsync
		{
			var state = await ChangeStateAsync<TState>();
			await state.EnterAsync();
		}

		public virtual async void EnterAsync<TState, TPayload>(TPayload payload) where TState : class, IPayloadStateAsync<TPayload>
		{
			var state = await ChangeStateAsync<TState>();
			await state.EnterAsync(payload);
		}

		protected virtual async Task<TState> ChangeStateAsync<TState>() where TState : class, IBaseState
		{
			if (CurrentState is IExitStateAsync exitState)
				await exitState.ExitAsync();

			var state = GetState<TState>();
			CurrentState = state;
			return state;
		}
	}
}