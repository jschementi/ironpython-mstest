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
