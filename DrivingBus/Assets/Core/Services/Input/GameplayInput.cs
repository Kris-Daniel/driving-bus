using UnityEngine;
using UnityEngine.InputSystem;

namespace Core.Services
{
	public class GameplayInput : InputWrapper
	{
		public InputActionEvents Look;
		Vector2 _lookValue;
		
		public InputActionEvents Forward;
		public InputActionEvents Backward;
		public InputActionEvents TurnLeft;
		public InputActionEvents TurnRight;
		public InputActionEvents Jump;
		public InputActionEvents Sprint;
		public InputActionEvents Interact;
		public InputActionEvents Reset;
		public InputActionEvents Attack;

		public GameplayInput(PlayerInput playerInput) : base(playerInput)
		{
			BindInputs();
		}

		public void BindInputs()
		{
			Forward = new InputActionEvents(_playerInput.actions["Forward"], this);
			Backward = new InputActionEvents(_playerInput.actions["Backward"], this);
			TurnLeft = new InputActionEvents(_playerInput.actions["TurnLeft"], this);
			TurnRight = new InputActionEvents(_playerInput.actions["TurnRight"], this);
			Look = new InputActionEvents(_playerInput.actions["Look"], this);
			Jump = new InputActionEvents(_playerInput.actions["Jump"], this);
			Sprint = new InputActionEvents(_playerInput.actions["Sprint"], this);
			Interact = new InputActionEvents(_playerInput.actions["Interact"], this);
			Reset = new InputActionEvents(_playerInput.actions["Reset"], this);
			Attack = new InputActionEvents(_playerInput.actions["Attack"], this);
			
			AddListener("Forward", Forward);
			AddListener("Backward", Backward);
			AddListener("TurnLeft", TurnLeft);
			AddListener("TurnRight", TurnRight);
			AddListener("Look", Look);
			AddListener("Jump", Jump);
			AddListener("Sprint", Sprint);
			AddListener("Interact", Interact);
			AddListener("Reset", Reset);
			AddListener("Attack", Attack);
		}
		
		public Vector2 MoveValue()
		{
			var fw = Forward.ReadValue<float>();
			var bw = Backward.ReadValue<float>();
			var left = TurnLeft.ReadValue<float>();
			var right = TurnRight.ReadValue<float>();
			var move = new Vector2(right - left, fw - bw);
			return move;
		}

		
		public Vector2 MoveRawValue()
		{
			var moveValue = MoveValue();
			return GetRawValue(moveValue);
		}

		Vector2 GetRawValue(Vector2 moveValue)
		{
			var treeshold = 0.01f;
			if (Mathf.Abs(moveValue.x) < treeshold)
			{
				moveValue.x = 0;
			}
			else
			{
				moveValue.x = Mathf.Sign(moveValue.x) * Mathf.Clamp01(Mathf.Abs(moveValue.x));
			}
			if (Mathf.Abs(moveValue.y) < treeshold)
			{
				moveValue.y = 0;
			}
			else
			{
				moveValue.y = Mathf.Sign(moveValue.y) * Mathf.Clamp01(Mathf.Abs(moveValue.y));
			}
			return moveValue;
		}
		
		public Vector2 LookValue()
		{
			return _lookValue;
		}
		
		public void SetLookValue(Vector2 lookValue)
		{
			_lookValue = lookValue;
		}
	}
}