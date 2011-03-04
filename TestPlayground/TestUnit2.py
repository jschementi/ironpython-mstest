import clr
clr.AddReference("Microsoft.VisualStudio.QualityTools.UnitTestFramework")
from Microsoft.VisualStudio.TestTools.UnitTesting import Assert

class TestUnit2(object):
    def test_should_pass(self):
        Assert.IsTrue(True)