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
