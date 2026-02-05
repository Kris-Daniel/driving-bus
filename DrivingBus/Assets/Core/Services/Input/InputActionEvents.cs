using System;
using UnityEngine.InputSystem;

namespace Core.Services
{
	public class InputActionEvents
	{
		public Action started = delegate { };
		public Action performed = delegate { };
		public Action canceled = delegate { };
		InputAction _action;
		InputWrapper _inputWrapper;

		public InputActionEvents(InputAction action, InputWrapper inputWrapper)
		{
			_inputWrapper = inputWrapper;
			_action = action;
		}

		public bool IsPressed()
		{
			return _inputWrapper.IsActive() && _action.IsPressed();
		}
		
		public TValue ReadValue<TValue>() where TValue : struct
		{
			return _inputWrapper.IsActive() ? _action.ReadValue<TValue>() : default;
		}

		public void CallStarted(InputAction.CallbackContext context)
		{
			if (_inputWrapper.IsActive())
			{
				started?.Invoke();
			}
		}
		
		public void CallPerformed(InputAction.CallbackContext context)
		{
			if (_inputWrapper.IsActive())
			{
				performed?.Invoke();
			}
		}
		
		public void CallCanceled(InputAction.CallbackContext context)
		{
			if (_inputWrapper.IsActive())
			{
				canceled?.Invoke();
			}
		}
	}
}