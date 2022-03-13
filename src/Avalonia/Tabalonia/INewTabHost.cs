using Avalonia;

namespace Tabalonia;

public interface INewTabHost<out TElement> where TElement : IAvaloniaObject
{
    TElement Container { get; }
    TabablzControl TabablzControl { get; }
}