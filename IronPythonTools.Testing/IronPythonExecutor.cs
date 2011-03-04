using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using IronPython.Runtime;
using IronPython.Runtime.Types;
using System.IO;
using System.Reflection;

namespace IronPythonTools.Testing
{
    public class IronPythonExecutor
    {
        internal ScriptEngine engine;

        internal ScriptScope module;

        public IronPythonExecutor(string inputFilePath, string inputFileContent)
        {
            engine = Python.CreateEngine();

            module = engine.CreateModule(
                Path.GetFileNameWithoutExtension(inputFilePath));

            engine.CreateScriptSourceFromString(
                inputFileContent,
                inputFilePath).Execute(module);
        }

        public List<List<string>> GetDeploymentItems(string configFilePath, string configFileContent)
        {
            var configModule = engine.CreateModule(configFilePath);
            engine.CreateScriptSourceFromString(configFileContent, configFilePath).Execute(configModule);
            List items = null;
            var deploymentItems = new List<List<string>>();
            if (configModule.TryGetVariable<List>("__deployment_items__", out items))
            {
                foreach (var item in items)
                {
                    string target = null;
                    string destination = null;

                    if (item as String != null)
                    {
                        destination = (string) item;
                    }
                    else if (item as PythonTuple != null)
                    {
                        var pt = (PythonTuple) item;
                        if (pt.Count > 1)
                        {
                            destination = (string) pt[0];
                            target = (string) pt[1];
                        } 
                        else if (pt.Count == 1)
                        {
                            destination = (string) pt[0];
                            target = destination.Split(new [] { "\\" }, StringSplitOptions.None).Last();
                        }
                    }

                    if (destination != null)
                        destination = destination.Replace("\\", "\\\\");

                    if (target != null)
                        target.Replace("\\", "\\\\");

                    deploymentItems.Add(new List<string> { destination, target });
                }
            }

            return deploymentItems;
        }

        public Dictionary<string, List<string>> GetStructure()
        {
            var classes = new Dictionary<string, List<string>>();

            var testClasses = from i in engine.Operations.GetMemberNames(module)
                              where i.StartsWith("Test")
                              where i.Length > 4
                              where i.Substring(4, 1) == i.Substring(4, 1).ToUpper()
                              select i;

            foreach (var className in testClasses)
            {
                object klass = engine.Operations.GetMember(module, className);

                classes.Add(
                    className,
                    engine.Operations.GetMemberNames(klass)
                        .Select(i => (string)i)
                        .Where(i => i.StartsWith("test_"))
                        .ToList<string>());
            }

            return classes;
        }
    }
}
