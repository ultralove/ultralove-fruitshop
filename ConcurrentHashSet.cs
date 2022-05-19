namespace Fruitshop
{
  public class ConcurrentHashSet<T> : IDisposable
  {
    private readonly HashSet<T> _items = new();
    private readonly ReaderWriterLockSlim _itemsLock = new(LockRecursionPolicy.SupportsRecursion);

    public HashSet<T> ToHashSet()
    {
      this._itemsLock.EnterReadLock();
      try {
        return this._items;
      }
      finally {
        if (this._itemsLock.IsReadLockHeld) {
          this._itemsLock.ExitReadLock();
        }
      }
    }

    #region Implementation of ICollection<T>
    public Boolean Add(T item)
    {
      this._itemsLock.EnterWriteLock();
      try {
        return this._items.Add(item);
      }
      finally {
        if (this._itemsLock.IsWriteLockHeld) {
          this._itemsLock.ExitWriteLock();
        }
      }
    }

    public Boolean AddRange(IEnumerable<T> items)
    {
      this._itemsLock.EnterWriteLock();
      try {
        var result = true;
        items.ToList().ForEach(item => {
          if (this._items.Add(item) == false) {
            result = false;
          }
        });
        return result;
      }
      finally {
        if (this._itemsLock.IsWriteLockHeld) {
          this._itemsLock.ExitWriteLock();
        }
      }
    }

    public void Clear()
    {
      this._itemsLock.EnterWriteLock();
      try {
        this._items.Clear();
      }
      finally {
        if (this._itemsLock.IsWriteLockHeld) {
          this._itemsLock.ExitWriteLock();
        }
      }
    }

    public Boolean Contains(T item)
    {
      this._itemsLock.EnterReadLock();
      try {
        return this._items.Contains(item);
      }
      finally {
        if (this._itemsLock.IsReadLockHeld) {
          this._itemsLock.ExitReadLock();
        }
      }
    }

    public Boolean Remove(T item)
    {
      this._itemsLock.EnterWriteLock();
      try {
        return this._items.Remove(item);
      }
      finally {
        if (this._itemsLock.IsWriteLockHeld) {
          this._itemsLock.ExitWriteLock();
        }
      }
    }

    public Int32 Count
    {
      get {
        this._itemsLock.EnterReadLock();
        try {
          return this._items.Count;
        }
        finally {
          if (this._itemsLock.IsReadLockHeld) {
            this._itemsLock.ExitReadLock();
          }
        }
      }
    }

    public List<T> ToList()
    {
      this._itemsLock.EnterReadLock();
      try {
        return this._items.ToList();
      }
      finally {
        if (this._itemsLock.IsReadLockHeld) {
          this._itemsLock.ExitReadLock();
        }
      }
    }
    #endregion

    #region Dispose
    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize(this);
    }
    protected virtual void Dispose(Boolean disposing)
    {
      if (disposing) {
        if (this._itemsLock != null) {
          this._itemsLock.Dispose();
        }
      }
    }
    ~ConcurrentHashSet()
    {
      this.Dispose(false);
    }
    #endregion
  }
}
