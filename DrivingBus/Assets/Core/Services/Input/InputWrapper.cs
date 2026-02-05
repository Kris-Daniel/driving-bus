using Core.Utils.Extensions;
using UnityEngine.InputSystem;

namespace Core.Services
{
	public class InputWrapper
	{
		protected PlayerInput _playerInput;
		bool _isActive;
		
		public InputWrapper(PlayerInput playerInput)
		{
			_playerInput = playerInput;
		}

		public void Activate() => _isActive = true;

		public void Deactivate() => _isActive = false;

		public bool IsActive() => _isActive;
		
		protected void AddListener(string actionName, InputActionEvents actionEvents)
		{
			_playerInput.actions[actionName].ClearCanceledEvents();
			_playerInput.actions[actionName].ClearPerformedEvents();
			_playerInput.actions[actionName].ClearStartedEvents();
			_playerInput.actions[actionName].performed += actionEvents.CallPerformed;
			_playerInput.actions[actionName].started += actionEvents.CallStarted;
			_playerInput.actions[actionName].canceled += actionEvents.CallCanceled;
		}
	}
}