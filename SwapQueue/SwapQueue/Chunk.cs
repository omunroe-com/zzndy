using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;

namespace SwapQueue
{
    internal class Chunk<T>
    {
        private string _path;
        private IEnumerable<T> _data;
        private readonly AutoResetEvent _handle;

        public Chunk( IEnumerable<T> data )
        {
            _data = data;
            _handle = new AutoResetEvent( false );

            Task.Factory.StartNew( Save );
        }

        private void Save()
        {
            try
            {
                do
                {
                    _path = Path.GetRandomFileName();
                } while ( File.Exists( _path ) );

                using ( FileStream straeam = File.OpenWrite( _path ) )
                {
                    var formatter = new BinaryFormatter();
                    formatter.Serialize( straeam, _data );
                    _data = null;
                }
            }
            finally
            {
                _handle.Set();
            }
        }

        public IEnumerable<T> Load()
        {
            _handle.WaitOne();

            var loadHandle = new AutoResetEvent( false );
            var obj = new DelayedCollection( loadHandle );
            Task.Factory.StartNew( () =>
                                       {
                                           try
                                           {
                                               using ( FileStream stream = File.OpenRead( _path ) )
                                               {
                                                   var formatter = new BinaryFormatter();
                                                   var collection = formatter.Deserialize( stream ) as IEnumerable<T>;
                                                   obj.Collection = collection;
                                               }
                                               File.Delete( _path );
                                           }
                                           finally
                                           {
                                               loadHandle.Set();
                                           }
                                       } );

            return obj;
        }

        private class DelayedCollection : IEnumerable<T>
        {
            private readonly AutoResetEvent _handle;
            private IEnumerable<T> _collection;

            public DelayedCollection( AutoResetEvent handle )
            {
                _handle = handle;
            }

            public IEnumerable<T> Collection
            {
                set { _collection = value; }
            }

            public IEnumerator<T> GetEnumerator()
            {
                _handle.WaitOne();

                return _collection == null ? new List<T>().GetEnumerator() : _collection.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }
}