
/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Reflection;
using EnvDTE90;
using Microsoft.Scripting.Hosting;
using System.IO;

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
            Dictionary<string, List<string>> structure,
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