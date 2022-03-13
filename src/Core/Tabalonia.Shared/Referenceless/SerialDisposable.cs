using System;

namespace Tabalonia.Core.Referenceless;

internal sealed class SerialDisposable : ICancelable
{
    private readonly object _gate = new();
    private IDisposable _current;
    private bool _disposed;

    /// <summary>
    /// Gets a value that indicates whether the object is disposed.
    /// 
    /// </summary>
    public bool IsDisposed
    {
        get
        {
            lock (this._gate)
                return this._disposed;
        }
    }

    /// <summary>
    /// Gets or sets the underlying disposable.
    /// 
    /// </summary>
    /// 
    /// <remarks>
    /// If the SerialDisposable has already been disposed, assignment to this property causes immediate disposal of the given disposable object. Assigning this property disposes the previous disposable object.
    /// </remarks>
    public IDisposable Disposable
    {
        get => this._current;
        set
        {
            bool flag = false;
            IDisposable disposable = null;
            lock (this._gate)
            {
                flag = this._disposed;
                if (!flag)
                {
                    disposable = this._current;
                    this._current = value;
                }
            }

            disposable?.Dispose();
            if (!flag || value == null)
                return;
            value.Dispose();
        }
    }

    /// <summary>
    /// Disposes the underlying disposable as well as all future replacements.
    /// 
    /// </summary>
    public void Dispose()
    {
        IDisposable disposable = null;
        lock (this._gate)
        {
            if (!this._disposed)
            {
                this._disposed = true;
                disposable = this._current;
                this._current = null;
            }
        }

        disposable?.Dispose();
    }
}