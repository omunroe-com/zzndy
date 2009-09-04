using System;
using System.IO;

using IAMPEEngine;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test_FiscalEngine
{
    /// <summary>
    ///This is a test class for EXE1000ATest and is intended
    ///to contain all EXE1000ATest Unit Tests
    ///</summary>
    [TestClass]
    public class GeneralCalculationTest
    {
        private TestContext testContextInstance;
        private const string pathDotnet = "dotnet";
        private const string pathOriginal = "original";

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes

        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        [TestInitialize]
        public void MyTestInitialize()
        {
            string dataPath = Path.GetFullPath( Path.Combine( Environment.CurrentDirectory, @"..\..\..\data" ) );
            string dataIn = Path.Combine( dataPath, "in" );
            string dataOut = Path.Combine( dataPath, "out" );

            Assert.IsTrue( Directory.Exists( dataPath ), "Data folder not found." );
            Assert.IsTrue( Directory.Exists( dataIn ), "Data input folder not found." );
            Assert.IsTrue( Directory.Exists( dataOut ), "Data output folder not found." );


            if ( Directory.Exists( pathDotnet ) )
            {
                Directory.Delete( pathDotnet );
            }
            if ( Directory.Exists( pathOriginal ) )
            {
                Directory.Delete( pathOriginal );
            }

            Assert.IsFalse( Directory.Exists( pathDotnet ), "Cannot setup fresh output folder for dotnet engine." );
            Assert.IsFalse( Directory.Exists( pathOriginal ), "Cannot setup fresh output folder for original engine." );

            Directory.CreateDirectory( pathDotnet );
            Directory.CreateDirectory( pathOriginal );

            Assert.IsTrue( Directory.Exists( pathDotnet ), "Failed to create working folder for dotnet engine." );
            Assert.IsTrue( Directory.Exists( pathOriginal ), "Failed to create working folder for original engine." );

            CopyDir( dataIn, pathDotnet );
            CopyDir( dataIn, pathOriginal );
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

        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //

        #endregion

        /// <summary>
        ///A test for Mainexec
        ///</summary>
        [TestMethod]
        public void CompareCalculationTest()
        {
            FiscalEngine.FiscalEngine dotnet = new FiscalEngine.FiscalEngine();
            AMPEEngine original = new AMPEEngine();

            foreach ( string directory in Directory.GetDirectories( pathDotnet ) )
            {
                string file = Path.GetFullPath( Path.Combine( directory, "~gnt1f.tmp" ) );
                dotnet.AsIMainexec.CalculateEconomics( file );
            }

            foreach ( string directory in Directory.GetDirectories( pathOriginal ) )
            {
                string file = Path.GetFullPath( Path.Combine( directory, "~gnt1f.tmp" ) );
                original.AsIMainexec.CalculateEconomics( file );
            }
        }
    }
}