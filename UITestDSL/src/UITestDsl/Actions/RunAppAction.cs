using System;
using System.Diagnostics;
using System.IO;

using Ranorex;

namespace UITestDsl.Actions
{
    internal class RunAppAction : BaseAction
    {
        private readonly string _fileDir;
        private readonly string _fileName;

        public RunAppAction( string path )
        {
            _fileDir = Path.GetDirectoryName( path );
            _fileName = Path.GetFileName( path );

            if ( !File.Exists( path ) )
            {
                throw new FileNotFoundException( String.Format( "File {0} not found. Could not run {1}", _fileName, path ) );
            }
        }

        public override string ToString()
        {
            return String.Format( "Run {0} in {1}", _fileName, _fileDir );
        }

        public override void Execute( IContext ctx )
        {
            Environment.CurrentDirectory =
                @"D:\workspace\enerdeq-tfs\branches\AssetBank_2.1_C7\AssetBank\build\bin\Debug\";
            var proc = Process.Start( "Enerdeq.exe" );

            var _form = Application.GetMainWindowFromProcess( proc );
            _form.Activate();

            ctx.AddForm( _form );
        }
    }
}