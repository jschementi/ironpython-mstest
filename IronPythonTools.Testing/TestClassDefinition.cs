using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IronPythonTools.Testing {
    public class TestClassDefinition {
        public string Name { get; set; }

        public string SetupMethodName { get; set; }

        public string TeardownMethodName { get; set; }

        public List<string> TestMethodNames { get; set; }

        public TestClassDefinition() {
            this.TestMethodNames = new List<string>();
        }
    }
}
