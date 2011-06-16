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

using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IronPythonTools.Testing.Tests {
    [TestClass]
    public class IronPythonTestingParserTest {
        private IronPythonTestingParser _target;
        private Dictionary<string, TestClassDefinition> _structure;

        [TestInitialize]
        public void TestInitialize() {
            _target = new IronPythonTestingParser("foo.py", @"
class TestFoo(object):
  def test_first(self): pass
  def test_second(self): pass
  def testnotatestmethod(self): pass
  def notatestmethod_test(self): pass
  def random(self): pass

class NotATestClass(object):
  def test_not_a_valid_test_method(self): pass

class TestWithSetupTeardown(object):
  def setUp(self): pass
  def test_foo(self): pass
  def tearDown(self): pass
");
            _structure = _target.GetStructure();
        }

        [TestMethod]
        public void GetStructureShouldGetValidTestClass() {
            Assert.IsTrue(_structure.ContainsKey("TestFoo"));
        }

        [TestMethod]
        public void GetStructureShouldNotGetInvalidTestClass() {
            Assert.IsFalse(_structure.ContainsKey("NotATestClass"));
        }

        [TestMethod]
        public void GetStructureShouldGetValidTestMethods() {
            CollectionAssert.Contains(_structure["TestFoo"].TestMethodNames, "test_first");
            CollectionAssert.Contains(_structure["TestFoo"].TestMethodNames, "test_second");
        }

        [TestMethod]
        public void GetStructureShouldNotGetInvalidTestMethods() {
            CollectionAssert.DoesNotContain(_structure["TestFoo"].TestMethodNames, "testnotatestmethod");
            CollectionAssert.DoesNotContain(_structure["TestFoo"].TestMethodNames, "notatestmethod_test");
            CollectionAssert.DoesNotContain(_structure["TestFoo"].TestMethodNames, "random");
            CollectionAssert.DoesNotContain(_structure["TestFoo"].TestMethodNames, "test_not_a_valid_test_method");
        }

        [TestMethod]
        public void GetStructureShouldDiscoverSetupAndTeardownMethods() {
            Assert.IsTrue(_structure.ContainsKey("TestWithSetupTeardown"));
            Assert.AreEqual("setUp", _structure["TestWithSetupTeardown"].SetupMethodName);
            Assert.AreEqual("tearDown", _structure["TestWithSetupTeardown"].TeardownMethodName);
        }
    }
}
