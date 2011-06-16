using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IronPythonTools.Testing
{
    public partial class TestPreprocessedTemplate
    {
        public string NameSpace { get; set; }
        public string[] Usings { get; set; }
        public string FileName { get; set; }
        public Dictionary<string, TestClassDefinition> Structure { get; set; }
        public List<List<string>> DeploymentItems { get; set; }
        public List<string> Path { get; set; }
    }
}
