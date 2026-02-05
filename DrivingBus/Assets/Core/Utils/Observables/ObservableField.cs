using System;
using UnityEngine;

namespace Core.Utils.Observables
{
	[Serializable]
	public class ObservableField<T> : IObservableValue<T>
	{
		[SerializeField] protected T _value;
		protected T _default;
		protected Action<T> _notify;

		public ObservableField(T initialValue)
		{
			_default = initialValue;
			_value = initialValue;
		}

		public event Action<T> OnValueChanged
		{
			add => _notify += value;
			remove => _notify -= value;
		}

		public T Value
		{
			get => _value;
			set
			{
				_value = value;
				_notify?.Invoke(value);
			}
		}

		public void ForceNotifyChangedValue()
		{
			bool isPlaying = false;
			
#if UNITY_EDITOR
			isPlaying = Application.isPlaying;
#endif
			
			if (isPlaying)
				_notify?.Invoke(_value);
		}

		public virtual void ResetValue()
		{
			Value = _default;
		}
	}
}