using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

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
            string workingPath = Path.GetFullPath( Environment.CurrentDirectory );

            CalculationConfigManager ccm = new CalculationConfigManager( dataPath, workingPath );

            ccm.SetupConfigs();

            testContextInstance.Properties.Add( "ccm", ccm );
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
            CalculationConfigManager ccm = testContextInstance.Properties[ "ccm" ] as CalculationConfigManager;
            if ( ccm == null )
            {
                Assert.Fail( "Configuration management was not correctly set up." );
                return; // Hint for resharper;
            }

            FiscalEngine.FiscalEngine dotnet = new FiscalEngine.FiscalEngine();
            AMPEEngine original = new AMPEEngine();

            Dictionary<string, bool> cases = new Dictionary<string, bool>();

            foreach ( CalculationCase calculationCase in ccm )
            {
                cases[ calculationCase.Name ] = true;

                switch ( calculationCase.Type )
                {
                    case ECalculationCaseType.DotNet:
                        dotnet.AsIMainexec.CalculateEconomics( calculationCase.StartFile );
                        break;
                    case ECalculationCaseType.Vb6:
                        original.AsIMainexec.CalculateEconomics( calculationCase.StartFile );
                        break;
                }
            }
            

            StringBuilder report = new StringBuilder();
            foreach ( string caseName in cases.Keys )
            {
                CalculationCase dotNetCase = ccm.GetCase( caseName, ECalculationCaseType.DotNet );
                CalculationCase originalCase = ccm.GetCase( caseName, ECalculationCaseType.Vb6 );

                const string fileName = "A2KRUN1.PRN";

                Report dotNetResult = CashflowReader.Read( "new", Path.Combine( dotNetCase.Location, fileName ) );
                Report originalResult = CashflowReader.Read( "old", Path.Combine( originalCase.Location, fileName ) );

                if(originalResult != dotNetResult)
                {
                    report.AppendFormat("Results for case {0} do not match. : {2}{1}{2}{2}", caseName,
                                         originalResult.GetLastInequalities(), Environment.NewLine );
                }
            }

            if(report.Length>0)Assert.Fail(report.ToString());
        }
    }
}