using Avalonia.Interactivity;

namespace Tabalonia;

public class RoutedPropertyChangedEventArgs<T> : RoutedEventArgs
{
    public RoutedPropertyChangedEventArgs(T oldValue, T newValue)
    {
        OldValue = oldValue;
        NewValue = newValue;
    }

    public RoutedPropertyChangedEventArgs(T oldValue, T newValue, RoutedEvent routedEvent)
        : this(oldValue, newValue)
    {
        RoutedEvent = routedEvent;
    }

    /// <summary>
    /// Return the old value
    /// </summary>
    public T OldValue { get; }

    /// <summary>
    /// Return the new value
    /// </summary>
    public T NewValue { get; }
}