using System;
using System.Diagnostics;
using System.Linq;

namespace UITestDsl
{
    public abstract class ErrorHandlingScanner : ScanBase
    {
        public override void yyerror( string format, params object[] args )
        {
            base.yyerror( format, args );

            throw new ApplicationException( format +
                                            String.Join( ", ", ( from arg in args select arg.ToString() ).ToArray() ) );
        }

        public void trace(string m)
        {
            Trace.WriteLine( m );
        }
    }
}