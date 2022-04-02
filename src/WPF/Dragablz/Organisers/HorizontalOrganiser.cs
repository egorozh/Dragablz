using System.Windows.Controls;

namespace Dragablz;

public sealed class HorizontalOrganiser : StackOrganiser
{
    public HorizontalOrganiser() : base(Orientation.Horizontal)
    { }

    public HorizontalOrganiser(double itemOffset) : base(Orientation.Horizontal, itemOffset)
    { }
}