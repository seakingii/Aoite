namespace System
{
    class SimpleLockItem : IDisposable
    {
        Action _disposing;
        public SimpleLockItem(Action disposing)
        {
            if(disposing == null) throw new ArgumentNullException(nameof(disposing));
            this._disposing = disposing;
        }

        void IDisposable.Dispose() => this._disposing();

        internal static void TimeoutError(string key, TimeSpan timeout)
        {
            throw new TimeoutException("键 {0} 的锁已被长时间占用，获取锁超时 {1}。".Fmt(key, timeout));
        }
    }
}
