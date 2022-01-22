using System;

namespace Tabalonia.Core.Referenceless;

internal static class Disposable
{
    public static IDisposable Empty => (IDisposable)DefaultDisposable.Instance;

    public static IDisposable Create(Action dispose)
    {
        if (dispose == null)
            throw new ArgumentNullException(nameof(dispose));

        return new AnonymousDisposable(dispose);
    }
}