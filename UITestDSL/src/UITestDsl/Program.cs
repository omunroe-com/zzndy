using System;
using System.Collections.Generic;
using System.IO;

using Action=UITestDsl.Actions.Action;

namespace UITestDsl
{
    internal class Program
    {
        [STAThread]
        private static void Main( string[] args )
        {
            string path = "../../../example.uit";
            Stream file = File.OpenRead( path );
            Scanner scanner = new Scanner( file );
            Parser parser = new Parser( scanner );
            bool res = parser.Parse();

            if ( res )
            {
                Queue<Action> actions = parser.GetActions();
                Tester tester = new Tester(actions);
                tester.RunScript();

                //Tester.RanorexMain( new string[] { } );
            }
        }
    }
}