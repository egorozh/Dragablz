using System;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Styling;

namespace Tabalonia;

public class DragablzIcon : TemplatedControl, IStyleable
{

    #region IStyleable

    Type IStyleable.StyleKey => typeof(DragablzIcon);

    #endregion

}