using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace station_mock_service_UnitTest
{
    [TestClass]
    public class UnitTest2
    {
        [TestMethod]
        public void DummyTestTrue()
        {
            string test = "test";
            Assert.AreEqual(test, "test");
        }
    }
    
}
