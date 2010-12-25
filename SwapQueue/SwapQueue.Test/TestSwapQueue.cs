using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SwapQueue.Test
{
    internal class NonSerializable
    {
    }

    [TestClass]
    public class TestSwapQueue
    {

        [TestMethod]
        [ExpectedException( typeof ( ApplicationException ), AllowDerivedTypes = true )]
        public void SerializableOnlyTest()
        {
            var neverQueue = new SwapQueue<NonSerializable>(2000);
            Console.WriteLine( neverQueue );
        }

        [TestMethod]
        public void AllItemsReturned()
        {
            var queue = new SwapQueue<int>( 10 );

            const int numTasks = 10;

            var tasks = new Task[numTasks];
            for ( int i = 0; i < numTasks; ++i )
            {
                int n = i;
                tasks[ i ] =Task.Factory.StartNew( () =>
                                               {
                                                   for ( int j = 0; j < 100; ++j )
                                                   {
                                                       queue.Enqueue( n * 100 + j );
                                                   }
                                               } );
            }

            Task.WaitAll( tasks );

            SortedDictionary<int, bool> numbers = new SortedDictionary<int, bool>();

            int item;
            while(queue.TryDequeue( out item ))
            {
                numbers[ item ] = true;
            }

            var missing = from number in numbers
                          where number.Value == false
                          select number.Key;

            Assert.AreEqual( 0, missing.Count(), "Queue lost data: " + String.Join( ", ", missing ) );
        }

        [TestMethod]
        public void MakeMillionByOne()
        {
            const int messages = 1000000;
            const int threads = 1;

            int n = CycleQueue( threads, messages );

            Assert.AreEqual( messages, n );
        }


        [TestMethod]
        public void MakeMillionByTen()
        {
            const int messages = 1000000;
            const int threads = 10;

            int n = CycleQueue( threads, messages );

            Assert.AreEqual( messages, n );
        }

        [TestMethod]
        public void MakeMillionByHundred()
        {
            const int messages = 1000000;
            const int threads = 100;

            int n = CycleQueue( threads, messages );

            Assert.AreEqual( messages, n );
        }


        private static int CycleQueue( int threads, int messages )
        {
            var tasks = new Task[threads];
            var queue = new SwapQueue<string>( 2000 );

            for ( int i = 0; i < threads; ++i )
            {
                tasks[ i ] = Task.Factory.StartNew( () =>
                                                        {
                                                            for ( int j = 0; j < messages / threads; ++j )
                                                            {
                                                                queue.Enqueue( Path.GetRandomFileName() );
                                                            }
                                                        } );
            }

            Task.WaitAll( tasks );

            int n = 0;
            string dummy;
            while ( queue.TryDequeue( out dummy ) )
            {
                ++n;
            }

            return n;
        }
    }
}