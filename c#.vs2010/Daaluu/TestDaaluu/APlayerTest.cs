using Daaluu.Logic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace TestDaaluu
{
    
    
    /// <summary>
    ///This is a test class for APlayerTest and is intended
    ///to contain all APlayerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class APlayerTest
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
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for getPairs
        ///</summary>
        [TestMethod()]
        public void getPairsTest()
        {
            List<ADomino> collection = new List<ADomino>(); // TODO: Initialize to an appropriate value
            collection.Add(DominoFactory.getADomino(DominoTypes.Daaluu));
            collection.Add(DominoFactory.getADomino(DominoTypes.Daaluu));
            List<ADomino> expected = new List<ADomino>(); // TODO: Initialize to an appropriate value
            ADomino tuple0 = DominoFactory.getADomino(DominoTypes.Daaluu);

            List<ADomino> actual;
            actual = APlayer.getPairs(collection);
            Assert.AreEqual(1, actual.Count);

            collection = new List<ADomino>(); // TODO: Initialize to an appropriate value
            collection.Add(DominoFactory.getADomino(DominoTypes.Daaluu));
            collection.Add(DominoFactory.getADomino(DominoTypes.Uuluu));
            collection.Add(DominoFactory.getADomino(DominoTypes.Daaluu));
            collection.Add(DominoFactory.getADomino(DominoTypes.Bajgar10));
            collection.Add(DominoFactory.getADomino(DominoTypes.Uuluu));
            collection.Add(DominoFactory.getADomino(DominoTypes.Yooz));
            collection.Add(DominoFactory.getADomino(DominoTypes.Vand4));
            collection.Add(DominoFactory.getADomino(DominoTypes.Buhuun4));
            expected = new List<ADomino>(); // TODO: Initialize to an appropriate value
            tuple0 = DominoFactory.getADomino(DominoTypes.Daaluu);

            actual = APlayer.getPairs(collection);
            Assert.AreEqual(2, actual.Count);
        }
    }
}
