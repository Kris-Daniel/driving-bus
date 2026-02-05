using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Core.Utils.Extensions
{
	public static class InputActionExtensions
	{
		public static void ClearPerformedEvents(this InputAction inputAction)
		{
			var field = typeof(InputAction).GetField("m_OnPerformed", BindingFlags.NonPublic | BindingFlags.Instance);
			if (field == null)
			{
				Debug.LogError("Field 'm_OnPerformed' not found in InputAction.");
				return;
			}
			if (field != null)
			{
				field.SetValue(inputAction, null);
			}
		}
		
		public static void ClearStartedEvents(this InputAction inputAction)
		{
			var field = typeof(InputAction).GetField("m_OnStarted", BindingFlags.NonPublic | BindingFlags.Instance);
			if (field == null)
			{
				Debug.LogError("Field 'm_OnStarted' not found in InputAction.");
				return;
			}
			if (field != null)
			{
				field.SetValue(inputAction, null);
			}
		}
		
		public static void ClearCanceledEvents(this InputAction inputAction)
		{
			var field = typeof(InputAction).GetField("m_OnCanceled", BindingFlags.NonPublic | BindingFlags.Instance);
			if (field == null)
			{
				Debug.LogError("Field 'm_OnCanceled' not found in InputAction.");
				return;
			}
			if (field != null)
			{
				field.SetValue(inputAction, null);
			}
		}
	}
}