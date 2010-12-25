using System;

namespace SwapQueue
{
    internal class Consumer<T> : IDisposable
    {
        private readonly SwapQueue<T> _queue = new SwapQueue<T>(2000);

        public void Consume( T item )
        {
            _queue.Enqueue( item );
        }

        public void Dispose()
        {
        }
    }
}