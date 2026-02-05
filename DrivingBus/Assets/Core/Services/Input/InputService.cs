using System.Collections.Generic;
using System.Linq;
using Core.Utils.Observables;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Core.Services
{
	public enum EInputSource
	{
		PC,
		Gamepad
	}
	
	public class InputService : MonoBehaviour
	{
		[SerializeField] PlayerInput _playerInput;

		public GameplayInput Gameplay { get; private set; }
		public ChangeButtonInput ChangeButtonInput { get; private set; }

		public ObservableField<EInputSource> InputSource = new ObservableField<EInputSource>(EInputSource.PC);

		Vector2 _lookFixedForFPS;

		public void Init()
		{
			Gameplay = new GameplayInput(_playerInput);
			ChangeButtonInput = new ChangeButtonInput(_playerInput);
			ChangeButtonInput.Activate();
		}

		public List<string> GetBindingPaths(string actionName)
		{
			List<string> bindingPaths = new List<string>();
			
			var action = _playerInput.actions[actionName];
			var actionBindings = action.bindings;
			foreach (var inputBinding in actionBindings)
			{
				var path = inputBinding.path;
				path = path.Replace("<", "/");
				path = path.Replace(">", "");
				bindingPaths.Add(path);
			}
			
			return bindingPaths;
		}

		void Update()
		{
			_lookFixedForFPS += Gameplay.Look.ReadValue<Vector2>();
			CheckIfLastInputSourceIsGamepadOrKeyboard();
		}

		void CheckIfLastInputSourceIsGamepadOrKeyboard()
		{
			if (Gamepad.current != null && Gamepad.current.allControls.Any(c => c.IsPressed()))
			{
				if(InputSource.Value != EInputSource.Gamepad)
					InputSource.Value = EInputSource.Gamepad;
				return;
			}

			// Check Keyboard keys pressed
			if (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame)
			{
				if(InputSource.Value != EInputSource.PC)
					InputSource.Value = EInputSource.PC;
				return;
			}

			// Check mouse buttons pressed (left, right, middle)
			if (Mouse.current != null)
			{
				if (Mouse.current.leftButton.wasPressedThisFrame ||
				    Mouse.current.rightButton.wasPressedThisFrame ||
				    Mouse.current.middleButton.wasPressedThisFrame)
				{
					if(InputSource.Value != EInputSource.PC)
						InputSource.Value = EInputSource.PC;
					return;
				}
			}
		}

		void FixedUpdate()
		{
			Gameplay.SetLookValue(_lookFixedForFPS);
			_lookFixedForFPS = Vector2.zero;
		}
	}
}