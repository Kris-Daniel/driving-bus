using System;

namespace Core.Utils.Observables
{
	public interface IObservableValue<out T>
	{
        /// <summary>
        ///     Value observed by hosts
        /// </summary>
        T Value { get; }

        /// <summary>
        ///     Happens when value changes
        /// </summary>
        event Action<T> OnValueChanged;
	}
}