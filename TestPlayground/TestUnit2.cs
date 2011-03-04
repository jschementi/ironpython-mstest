using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace TestPlayground
{
    [TestClass]
    public class TestUnit2
    {
        private DynamicUnitTestEngine testEngine;

        public TestUnit2()
        {
            testEngine = DynamicUnitTestEngine.Create("TestUnit2", "TestUnit2.py");
        }

        [TestInitialize]
        public void setUp() {
            testEngine.InvokeSetup();
        }

        [TestCleanup]
        public void tearDown() {
            testEngine.InvokeTeardown();
        }

        [TestMethod()]
#if !SILVERLIGHT
        [DeploymentItem("TestUnit2.py")]
#endif
        public void test_should_pass() {
            testEngine.InvokeTestMethod("test_should_pass");
        }
    }
}