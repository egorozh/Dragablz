using Avalonia.Layout;

namespace Avalonium.Organisers;

public class HorizontalOrganiser : StackOrganiser
{
    public HorizontalOrganiser() : base(Orientation.Horizontal)
    { }

    public HorizontalOrganiser(double itemOffset) : base(Orientation.Horizontal, itemOffset)
    { }
}