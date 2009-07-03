using System;
using System.Threading;

namespace UITestDsl.Actions
{
    internal class WaitAction : BaseAction
    {
        private readonly int _timeout;

        public WaitAction( int timeout )
        {
            _timeout = timeout;
        }

        public override string ToString()
        {
            return String.Format( "Wait for {0}ms", _timeout );
        }

        public override void Execute( IContext ctx )
        {
            Thread.Sleep( _timeout );
        }
    }
}