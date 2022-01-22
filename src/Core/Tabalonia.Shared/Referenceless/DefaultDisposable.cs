using System;

namespace Tabalonia.Core.Referenceless;

internal sealed class DefaultDisposable : IDisposable
{
    public static readonly DefaultDisposable Instance = new();

    static DefaultDisposable()
    {
    }

    private DefaultDisposable()
    {
    }

    public void Dispose()
    {
    }
}