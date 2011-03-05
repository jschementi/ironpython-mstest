import clr
clr.AddReference("Microsoft.VisualStudio.QualityTools.UnitTestFramework")
from Microsoft.VisualStudio.TestTools.UnitTesting import Assert

class TestFoo(object):
    def test_should_pass(self):
        Assert.IsTrue(True)
    def test_should_pass2(self):
        Assert.IsTrue(True)
    def test_should_fail(self):
        Assert.IsTrue(False)
    
