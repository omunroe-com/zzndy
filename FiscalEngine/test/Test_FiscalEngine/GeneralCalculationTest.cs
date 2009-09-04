using System;
using System.Collections.Generic;
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

            string dataPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\..\data"));
            string workingPath = Path.GetFullPath(Environment.CurrentDirectory);

            CalculationConfigManager ccm = new CalculationConfigManager(dataPath, workingPath);

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
            var ccm = testContextInstance.Properties[ "ccm" ] as CalculationConfigManager;
            if(ccm==null)Assert.Fail("Configuration management was not correctly set up.");

            FiscalEngine.FiscalEngine dotnet = new FiscalEngine.FiscalEngine();
            AMPEEngine original = new AMPEEngine();

            foreach ( CalculationCase calculationCase in ccm )
            {
                switch ( calculationCase.Type )
                {
                    case ECalculationCaseType.DotNet:
                        dotnet.AsIMainexec.CalculateEconomics( calculationCase.StartFile );
                        break;
                    case ECalculationCaseType.Vb6:
                        original.AsIMainexec.CalculateEconomics(calculationCase.StartFile);
                        break;
                }
            }
        }
    }
}