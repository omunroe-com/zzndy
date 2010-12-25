using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace SwapQueue
{
    internal class Swap<T>
    {
        private ConcurrentQueue<Chunk<T>> _chunks;

        public Swap()
        {
            if ( !typeof ( T ).IsSerializable )
            {
                throw new ApplicationException( GetType().Name +
                                                " cannot operate with instances of nonserializable class " +
                                                typeof ( T ).Name );
            }

            _chunks = new ConcurrentQueue<Chunk<T>>();
        }

        public void Save( IEnumerable<T> chunkData )
        {
            var chunk = new Chunk<T>( chunkData );
            _chunks.Enqueue( chunk );
        }

        public IEnumerable<T> Load()
        {
            Chunk<T> chunk;
            bool haveChunk = _chunks.TryDequeue( out chunk );

            return haveChunk ? chunk.Load() : null;
        }
    }
}