using System;
using Avalonia;

namespace Tabalonia;

internal class ContainerCustomisations
{
    public Func<DragablzItem>? GetContainerForItemOverride { get; }

    public Action<IAvaloniaObject, object>? PrepareContainerForItemOverride { get; }

    public Action<IAvaloniaObject, object>? ClearingContainerForItemOverride { get; }

    public ContainerCustomisations(Func<DragablzItem>? getContainerForItemOverride = null,
        Action<IAvaloniaObject, object>? prepareContainerForItemOverride = null,
        Action<IAvaloniaObject, object>? clearingContainerForItemOverride = null)
    {
        GetContainerForItemOverride = getContainerForItemOverride;
        PrepareContainerForItemOverride = prepareContainerForItemOverride;
        ClearingContainerForItemOverride = clearingContainerForItemOverride;
    }
}