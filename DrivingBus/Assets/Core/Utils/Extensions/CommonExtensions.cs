using UnityEngine;

namespace Core.Utils.Extensions
{
	public static class CommonExtensions
	{
		public static float Remap(this float value, float from1, float to1, float from2, float to2)
		{
			return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
		}
		
		public static float RemapClamped(this float value, float fromMin, float fromMax, float toMin, float toMax)
		{
			// Determine actual min and max values for both ranges
			float actualFromMin = Mathf.Min(fromMin, fromMax);
			float actualFromMax = Mathf.Max(fromMin, fromMax);
			float actualToMin = Mathf.Min(toMin, toMax);
			float actualToMax = Mathf.Max(toMin, toMax);

			// Clamp input value to source range first
			float clampedInput = Mathf.Clamp(value, actualFromMin, actualFromMax);

			// Calculate the remapped value using absolute min/max
			float remapped;
			if (actualFromMax - actualFromMin == 0) // Prevent division by zero
			{
				remapped = actualToMin; // If source range is zero, return target minimum
			}
			else
			{
				// Standard remapping formula with direction handling
				float fromRange = actualFromMax - actualFromMin;
				float toRange = actualToMax - actualToMin;
				float progress = (clampedInput - actualFromMin) / fromRange;
            
				// If target range is inverted compared to source range, reverse the progress
				if ((fromMax < fromMin) != (toMax < toMin))
				{
					progress = 1f - progress;
				}
            
				remapped = actualToMin + (progress * toRange);
			}

			// Final clamp to target range (mostly for floating-point precision safety)
			return Mathf.Clamp(remapped, actualToMin, actualToMax);
		}
		
		public static void DrawCircle(Vector3 position, float radius, Vector3 up, int segments, Color color, float duration = 0f)
		{
			if (segments < 3) segments = 3; // Minimum segments to form a shape
    
			float angleStep = 360f / segments;
			Vector3 right = Vector3.Cross(up, Vector3.forward).normalized;
			if (right == Vector3.zero) right = Vector3.Cross(up, Vector3.up).normalized;
			Vector3 forward = Vector3.Cross(right, up).normalized;
    
			Vector3[] points = new Vector3[segments + 1];
    
			// Calculate points around the circle
			for (int i = 0; i <= segments; i++)
			{
				float angle = i * angleStep * Mathf.Deg2Rad;
				Vector3 point = position + 
				                (right * Mathf.Cos(angle) + forward * Mathf.Sin(angle)) * radius;
				points[i] = point;
			}
    
			// Draw lines between points
			for (int i = 0; i < segments; i++)
			{
				Debug.DrawLine(points[i], points[i + 1], color, duration);
			}
		}

	}
}