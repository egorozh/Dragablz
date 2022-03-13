using System;

namespace Tabalonia.Core.Referenceless;

internal interface ICancelable : IDisposable
{
    bool IsDisposed { get; }
}