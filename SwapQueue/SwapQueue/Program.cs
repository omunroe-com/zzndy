using System;
using System.Threading;

namespace SwapQueue
{
    internal class Program
    {
        private static void Main( string[] args )
        {
            const ushort threads = 10;
            const int workSeconds = 10;
            Console.WriteLine( "Starting generation in {0} threads for {1} seconds.", threads, workSeconds );

            using ( var consumer = new Consumer<string>() )
            {
                var producer = new Producer<string>( consumer, threads );

                producer.Start();

                Thread.Sleep( workSeconds * 1000 );

                Console.WriteLine( "Time to go" );
                producer.Stop();
            }

            Console.WriteLine( "Done" );
        }
    }
}