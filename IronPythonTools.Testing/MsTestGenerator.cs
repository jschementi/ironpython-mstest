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

using System;
using System.Collections.Generic;
using System.Linq;

namespace IronPythonTools.Testing
{
    public static class MsTestGenerator
    {
        private static string MakeRelativeTo(string path, string basePath)
        {
            return new Uri(basePath).MakeRelativeUri(new Uri(path)).ToString();
        }

        public static string Generate(
            string inputFileName,
            string projectFileName,
            Dictionary<string, TestClassDefinition> structure,
            string @namespace,
            List<List<string>> deploymentItems)
        {
            var relativeInputFileName = MakeRelativeTo(inputFileName, projectFileName);

            deploymentItems.Add(new List<string> { relativeInputFileName, null });

            var path = deploymentItems.Select(i => i.Count > 1 ? i[1] : null).Where(i => i != null).ToList();

            var usings = new[]
                         {
                             "System",
                             "System.Collections.Generic",
                             "IronPython.Hosting",
                             "Microsoft.Scripting.Hosting",
                             "Microsoft.VisualStudio.TestTools.UnitTesting",
                             "System.IO",
                             "System.Reflection",
                         };

            var t = new TestPreprocessedTemplate
                    {
                        NameSpace = @namespace,
                        Usings = usings,
                        FileName = relativeInputFileName,
                        Structure = structure,
                        DeploymentItems = deploymentItems,
                        Path = path
                    };

            return t.TransformText();
        }
    }
}