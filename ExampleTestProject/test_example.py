import clr
clr.AddReference("Microsoft.VisualStudio.QualityTools.UnitTestFramework")
from Microsoft.VisualStudio.TestTools.UnitTesting import Assert

class TestFoo(object):
    
    def setUp(self):
        self.Hi = "Jimmy"
    
    def test_should_pass(self):
        Assert.IsTrue(self.Hi == "Jimmy")

    def test_should_not_fail(self):
        Assert.IsTrue(True)
