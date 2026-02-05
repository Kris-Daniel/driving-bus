using UnityEngine.InputSystem;

namespace Core.Services
{
	public class ChangeButtonInput : InputWrapper
	{
		public InputActionEvents SelectInput;
		public InputActionEvents BackInput;
		public InputActionEvents AnyKeyboardKey;
		
		public ChangeButtonInput(PlayerInput playerInput) : base(playerInput)
		{
			SelectInput = new InputActionEvents(playerInput.actions["SelectInput"], this);
			BackInput = new InputActionEvents(playerInput.actions["BackInput"], this);
			
			AddListener("SelectInput", SelectInput);
			AddListener("BackInput", BackInput);
		}
	}
}