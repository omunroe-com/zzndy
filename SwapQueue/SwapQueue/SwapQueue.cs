using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace SwapQueue
{
    /// <summary>
    ///   Represents a queue of <typeparamref name = "T" /> wich is saved to the disk should 
    ///   the number of items grow too fast.
    /// </summary>
    /// <typeparam name = "T"></typeparam>
    public class SwapQueue<T>
    {
        private readonly int _shardCountThreshold;
        private ConcurrentQueue<T> _queue;
        private IEnumerable<T> _hotSwap;
        private readonly Swap<T> _swap;
        private readonly object _queueSwitchHandle = new object();

        /// <summary>
        ///   Initialize a new instance of <see cref = "SwapQueue{T}" />.
        /// </summary>
        /// <exception cref = "ApplicationException">Thrown if <typeparamref name = "T" /> is not serializable.</exception>
        public SwapQueue( int shardCountThreshold )
        {
            if ( !typeof ( T ).IsSerializable )
            {
                throw new ApplicationException( GetType().Name +
                                                " cannot operate with instances of nonserializable class " +
                                                typeof ( T ).Name );
            }

            _queue = new ConcurrentQueue<T>();
            _swap = new Swap<T>();
            _shardCountThreshold = shardCountThreshold;
        }

        /// <summary>
        ///   Attempt to remove and return the object at the beginnign of <see cref = "SwapQueue{T}" />.
        /// </summary>
        /// <param name = "item">Contains the object returned if operation successfull.</param>
        /// <returns>True, if an object was returned from the queue, false otherwise.</returns>
        public bool TryDequeue( out T item )
        {
            bool returned = _queue.TryDequeue( out item );

            if ( !returned )
            {
                lock ( _queueSwitchHandle )
                {
                    returned = _queue.TryDequeue( out item );
                    if ( !returned )
                    {
                        if ( _hotSwap != null )
                        {
                            _queue = new ConcurrentQueue<T>( _hotSwap );
                            returned = _queue.TryDequeue( out item );

                            _hotSwap = _swap.Load();
                        }
                    }
                }
            }

            return returned;
        }

        /// <summary>
        ///   Add an object to <see cref = "SwapQueue{T}" />.
        /// </summary>
        /// <param name = "item"></param>
        public void Enqueue( T item )
        {
            _queue.Enqueue( item );

            if ( _queue.Count > _shardCountThreshold )
            {
                lock ( _queueSwitchHandle )
                {
                    if ( _queue.Count > _shardCountThreshold )
                    {
                        if ( _hotSwap != null )
                        {
                            _swap.Save( _hotSwap );
                        }

                        _hotSwap = _queue;
                        _queue = new ConcurrentQueue<T>();
                    }
                }
            }
        }
    }
}