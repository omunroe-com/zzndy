using System;
using System.Threading;
using System.Threading.Tasks;

namespace SwapQueue
{
    internal class Producer<T>
    {
        private readonly Consumer<T> _consumer;

        private readonly ushort _numThreads;

        private CancellationTokenSource _src;
        private Task[] _tasks;

        public Producer( Consumer<T> consumer, ushort numThreads )
        {
            _consumer = consumer;

            _numThreads = numThreads;
        }

        public void Start()
        {
            _src = new CancellationTokenSource();
            _tasks = new Task[_numThreads];

            CancellationToken tok = _src.Token;
            for ( int i = 0; i < _numThreads; ++i )
            {
                int n = i;
                _tasks[ i ] = Task.Factory.StartNew( () =>
                                                         {
                                                             while ( !tok.IsCancellationRequested )
                                                             {
                                                                 _consumer.Consume( MakeMore() );
                                                             }
                                                             if ( n == 5 )
                                                             {
                                                                 throw new ApplicationException( "HELLO" );
                                                             }
                                                             tok.ThrowIfCancellationRequested();

                                                             Console.WriteLine( "Thread finished." );
                                                         }, tok );
            }
        }

        public void Stop()
        {
            _src.Cancel();
            try
            {
                Console.WriteLine( "Waiting for generators to complete ..." );
                Task.WaitAll( _tasks, _src.Token );
                Console.WriteLine( "All generators quietly completed." );
            }
            catch ( OperationCanceledException )
            {
            }
            catch ( AggregateException e )
            {
                foreach ( Exception innerException in e.InnerExceptions )
                {
                    if ( !( innerException is OperationCanceledException ) )
                    {
                        Console.Error.WriteLine( "Error: Task generated exception: " + innerException.Message );
                    }
                }
            }
        }

        private static T MakeMore()
        {
            return ( T ) ( object ) "Message";
        }
    }
}