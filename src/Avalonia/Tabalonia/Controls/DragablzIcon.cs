using Avalonia.Controls.Primitives;
using Avalonia.Styling;
using System;

namespace Tabalonia;

public class DragablzIcon : TemplatedControl, IStyleable
{
    #region IStyleable

    Type IStyleable.StyleKey => typeof(DragablzIcon);

    #endregion
}