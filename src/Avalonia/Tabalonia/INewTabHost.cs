namespace Tabalonia;

public interface INewTabHost<out TElement> where TElement : UIElement
{
    TElement Container { get; }
    TabablzControl TabablzControl { get; }
}