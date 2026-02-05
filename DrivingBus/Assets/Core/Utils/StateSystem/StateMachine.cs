using System;
using System.Collections.Generic;
using Core.Utils.StateSystem.Interfaces;
using UnityEngine;

namespace Core.Utils.StateSystem
{
	[Serializable]
	public class StateMachine
	{
		private readonly Dictionary<Type, IBaseState> _states;
		private readonly Dictionary<Type, IUpdateState> _updatableStates;

		public bool IsDebugMode;

		public StateMachine(params IBaseState[] states)
		{
			_states = new Dictionary<Type, IBaseState>();
			_updatableStates = new Dictionary<Type, IUpdateState>();

			foreach (var state in states)
			{
				if (state is IUpdateState updatableState && !_updatableStates.ContainsKey(updatableState.GetType()))
					_updatableStates.Add(updatableState.GetType(), updatableState);

				if (_states.ContainsKey(state.GetType()))
					continue;

				state.StateMachine = this;
				_states.Add(state.GetType(), state);
			}
		}

		protected IBaseState CurrentState { get; set; }

		public bool IsCurrentStateOfType<T>()
		{
			return CurrentState is T;
		}

		public virtual void Enter<TState>() where TState : class, IState
		{
			var state = ChangeState<TState>();
			if (IsDebugMode)
			{
				Debug.Log("Enter " + state.GetType().Name);
			}
			state.Enter();
		}

		public virtual void Enter<TState, TPayload>(TPayload payload) where TState : class, IPayloadState<TPayload>
		{
			var state = ChangeState<TState>();
			if (IsDebugMode)
			{
				Debug.Log("Enter " + state.GetType().Name);
			}
			state.Enter(payload);
		}

		protected virtual TState ChangeState<TState>() where TState : class, IBaseState
		{
			if (CurrentState != null)
				CurrentState.isActive = false;

			if (CurrentState is IExitState exitState)
			{
				if (IsDebugMode)
				{
					Debug.Log("Exit " + exitState.GetType().Name);
				}
				exitState.Exit();
			}

			var state = GetState<TState>();
			state.isActive = true;
			CurrentState = state;
			return state;
		}

		protected virtual TState GetState<TState>() where TState : class, IBaseState
		{
			return _states[typeof(TState)] as TState;
		}

		public void UpdateTick()
		{
			foreach (var state in _updatableStates.Values)
				state.Update();
		}
		
		public void FixedUpdateTick()
		{
			foreach (var state in _updatableStates.Values)
				state.Update();
		}
	}
}