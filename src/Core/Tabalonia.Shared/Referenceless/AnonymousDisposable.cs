using System;
using System.Threading;

namespace Tabalonia.Core.Referenceless;

internal sealed class AnonymousDisposable : ICancelable
{
    private volatile Action _dispose;

    public bool IsDisposed => this._dispose == null;

    public AnonymousDisposable(Action dispose)
    {
        this._dispose = dispose;
    }

    public void Dispose()
    {
        var action = Interlocked.Exchange(ref _dispose, null);
        action?.Invoke();
    }
}