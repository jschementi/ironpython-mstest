// Copyright 2011 Jimmy Schementi
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

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