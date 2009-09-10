using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test_FiscalEngine
{
    /// <summary>
    /// Sets up files required to run calculation.
    /// </summary>
    internal class CalculationConfigManager : IEnumerable<CalculationCase>
    {
        private const string PathDotnet = "dotnet";
        private const string PathOriginal = "original";

        private readonly string _dataPath;
        private readonly string _testPath;
        private SortedDictionary<string, CalculationCase> _cases;

        [DllImport( "kernel32.dll", CharSet = CharSet.Auto )]
        public static extern int GetShortPathName(
            [MarshalAs( UnmanagedType.LPTStr )] string path,
            [MarshalAs( UnmanagedType.LPTStr )] StringBuilder shortPath,
            int shortPathLength
            );


        /// <summary>
        /// Initializes an instnce of <see cref="CalculationConfigManager"/> with source 
        /// and destination paths.
        /// </summary>
        /// <param name="dataPath">Location where reference input and outpuut files are located.</param>
        /// <param name="testPath">Location where actual calculation configs will be located.</param>
        public CalculationConfigManager( string dataPath, string testPath )
        {
            if ( String.IsNullOrEmpty( dataPath ) )
            {
                throw new ArgumentNullException( "dataPath" );
            }

            if ( String.IsNullOrEmpty( testPath ) )
            {
                throw new ArgumentNullException( "testPath" );
            }

            if ( !Directory.Exists( dataPath ) )
            {
                throw new ArgumentException( "dataPath" );
            }

            if ( !Directory.Exists( testPath ) )
            {
                throw new ArgumentException( "testPath" );
            }

            _dataPath = dataPath;
            _testPath = testPath;
        }

        /// <summary>
        /// Setup configs for calculation.
        /// </summary>
        public void SetupConfigs()
        {
            PrepareLayout();
            PrepareFiles();
        }

        /// <summary>
        /// Copy all necessary configs to destination folder.
        /// </summary>
        private void PrepareLayout()
        {
            string dataIn = Path.Combine( _dataPath, "in" );
            string dataOut = Path.Combine( _dataPath, "out" );

            Assert.IsTrue( Directory.Exists( _dataPath ), "Data folder not found." );
            Assert.IsTrue( Directory.Exists( dataIn ), "Data input folder not found." );
            Assert.IsTrue( Directory.Exists( dataOut ), "Data output folder not found." );


            if ( Directory.Exists( PathDotnet ) )
            {
                Directory.Delete( PathDotnet );
            }
            if ( Directory.Exists( PathOriginal ) )
            {
                Directory.Delete( PathOriginal );
            }

            Assert.IsFalse( Directory.Exists( PathDotnet ), "Cannot setup fresh output folder for dotnet engine." );
            Assert.IsFalse( Directory.Exists( PathOriginal ), "Cannot setup fresh output folder for original engine." );

            Directory.CreateDirectory( PathDotnet );
            Directory.CreateDirectory( PathOriginal );

            Assert.IsTrue( Directory.Exists( PathDotnet ), "Failed to create working folder for dotnet engine." );
            Assert.IsTrue( Directory.Exists( PathOriginal ), "Failed to create working folder for original engine." );

            CopyDir( dataIn, PathDotnet );
            CopyDir( dataIn, PathOriginal );
        }

        private static void CopyDir( string src, string dst )
        {
            if ( !Directory.Exists( dst ) )
            {
                Directory.CreateDirectory( dst );
            }

            foreach ( string file in Directory.GetFiles( src ) )
            {
                string filename = Path.GetFileName( file );
                File.Copy( file, Path.Combine( dst, filename ) );
            }

            foreach ( string dir in Directory.GetDirectories( src ) )
            {
                string dirname = Path.GetFileName( dir );
                if ( dirname.StartsWith( "." ) )
                {
                    continue;
                }

                CopyDir( dir, Path.Combine( dst, dirname ) );
            }
        }

        /// <summary>
        /// Alter configs for proper calculation.
        /// </summary>
        private void PrepareFiles()
        {
            _cases = new SortedDictionary<string, CalculationCase>();

            ReadCases( PathDotnet );
            ReadCases( PathOriginal );

            foreach ( CalculationCase calculationCase in this )
            {
                PutWorkingDir( calculationCase, "GNTCONFG.DAT", 5, 6 );
                PutWorkingDir( calculationCase, "RUNNAME.PRN", 0 );

                SetupA2KRun( calculationCase, _dataPath );
                SetupStartup( calculationCase, _testPath );
            }
        }

        private static void SetupStartup( CalculationCase calculationCase, string testPath )
        {
            const string name = "GiExStat.tmp";

            using ( TextWriter w = new StreamWriter( calculationCase.StartFile ) )
            {
                w.WriteLine( "\"" + GetShortPath( testPath ) + "\\\"" );
                w.WriteLine( "\"" + GetShortPath( calculationCase.Location ) + "\\\"" );
                w.WriteLine( "\"" + GetShortPath( calculationCase.Location ) + "\\\"" );
                w.WriteLine( "\"" + GetShortPath( Path.Combine( calculationCase.Location, name ) ) + "\"" );
            }
        }

        private static void SetupA2KRun( CalculationCase calculationCase, string dataPath )
        {
            string path = Path.Combine( calculationCase.Location, "A2KRUN.RUN" );
            FileInfo fi = new FileInfo( path );

            string[] lines;
            using ( TextReader r = new StreamReader( fi.OpenRead() ) )
            {
                lines = r.ReadToEnd().Split( new[] { '\n' } );
            }

            string[] parts = lines[ 2 ].Split( ',' );
            parts[ 1 ] = "\"" + GetShortPath( calculationCase.Location ) + "\\\"";
            lines[ 2 ] = String.Join( ",", parts );

            parts = lines[ 3 ].Split( ',' );
            parts[ 1 ] = "\"" + GetShortPath( dataPath ) + "\\\"";
            lines[ 3 ] = String.Join( ",", parts );

            using ( TextWriter w = new StreamWriter( fi.OpenWrite() ) )
            {
                w.Write( String.Join( "\n", lines ) );
            }
        }

        private static void PutWorkingDir( CalculationCase calculationCase, string fileName, params int[] ids )
        {
            string path = Path.Combine( calculationCase.Location, fileName );

            FileInfo fi = new FileInfo( path );

            string line;
            using ( TextReader r = new StreamReader( fi.OpenRead() ) )
            {
                line = r.ReadLine();
            }

            string[] parts = line.Split( ',' );
            string v = "\"" + GetShortPath( calculationCase.Location ) + "\\\"";

            foreach ( int i in ids )
            {
                parts[ i ] = v;
            }

            using ( TextWriter w = new StreamWriter( fi.OpenWrite() ) )
            {
                w.WriteLine( String.Join( ",", parts ) );
            }
        }

        private static string GetShortPath( string path, params string[] parts )
        {
            foreach ( string pathPart in parts )
            {
                path = Path.Combine( path, pathPart );
            }

            StringBuilder shortPath = new StringBuilder( 255 );
            GetShortPathName( path, shortPath, shortPath.Capacity );

            return shortPath.ToString();
        }

        private void ReadCases( string path )
        {
            foreach ( string directory in Directory.GetDirectories( Path.Combine( _testPath, path ) ) )
            {
                ECalculationCaseType type = path == PathDotnet ? ECalculationCaseType.DotNet : ECalculationCaseType.Vb6;
                _cases.Add( type + "/" + Path.GetFileName(directory), new CalculationCase( directory, type ) );
            }
        }

        #region Implementation of IEnumerable

        public IEnumerator<CalculationCase> GetEnumerator()
        {
            return _cases.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        public CalculationCase GetCase( string name, ECalculationCaseType type )
        {
            CalculationCase @case = _cases[ type + "/" + name ];
            return @case;
        }
    }
}