using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using IronPython;
using IronPython.Compiler;
using IronPython.Compiler.Ast;
using IronPython.Hosting;
using IronPython.Runtime;

using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using Microsoft.Scripting.Hosting.Providers;
using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;

namespace IronPythonTools.Testing {
    public class IronPythonTestingParser {
        private readonly PythonAst _ast;

        private readonly ScriptEngine _engine = Python.CreateEngine();

        public IronPythonTestingParser(string inputFilePath, string inputFileContent) {
            _ast = GetAst(_engine, inputFilePath, inputFileContent);
        }

        public List<List<string>> GetDeploymentItems(string configFilePath, string configFileContent) {
            var configModule = _engine.CreateModule(configFilePath);
            _engine.CreateScriptSourceFromString(configFileContent, configFilePath)
                   .Execute(configModule);

            var deploymentItems = new List<List<string>>();
            List items;
            if (configModule.TryGetVariable("__deployment_items__", out items)) {
                foreach (var item in items) {
                    string target = null;
                    string destination = null;

                    if (item as String != null) {
                        destination = (string)item;
                    } else if (item as PythonTuple != null) {
                        var pt = (PythonTuple)item;
                        if (pt.Count > 1) {
                            destination = (string)pt[0];
                            target = (string)pt[1];
                        } else if (pt.Count == 1) {
                            destination = (string)pt[0];
                            target = destination.Split(new[] { "\\" }, StringSplitOptions.None).Last();
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

        public Dictionary<string, TestClassDefinition> GetStructure() {
            var walker = new StructureWalker();
            _ast.Walk(walker);
            return walker.Structure;
        }

        private static PythonAst GetAst(ScriptEngine engine, string filePath, string fileContents) {
            if (Path.GetExtension(filePath) != ".py") {
                return null;
            }

            var sourceUnit = HostingHelpers.GetSourceUnit(engine.CreateScriptSourceFromString(fileContents, filePath));
            var languageContext = HostingHelpers.GetLanguageContext(engine);

            var parser =
                Parser.CreateParser(
                    new CompilerContext(sourceUnit, languageContext.GetCompilerOptions(), ErrorSink.Default),
                    languageContext.Options as PythonOptions);

            // will throw on syntax error ...
            return parser.ParseFile(true);
        }

        private class StructureWalker : PythonWalker {
            private TestClassDefinition _currentClass;
            private readonly string _classPrefix = "Test";
            private readonly string _methodPrefix = "test_";
            private readonly string _setupName = "setUp";
            private readonly string _teardownName = "tearDown";

            public Dictionary<string, TestClassDefinition> Structure { get; private set; }

            public StructureWalker() {
                Structure = new Dictionary<string, TestClassDefinition>();
            }

            public StructureWalker(string classPrefix, string methodPrefix)
                : this() {
                _classPrefix = classPrefix;
                _methodPrefix = methodPrefix;
            }

            public override bool Walk(ClassDefinition node) {
                if (IsTestClass(node)) {
                    _currentClass = new TestClassDefinition { Name = node.Name };
                    Structure.Add(_currentClass.Name, _currentClass);
                    return base.Walk(node);
                }

                return false;
            }

            public override void PostWalk(ClassDefinition node) {
                if (_currentClass == null) return;

                ContractUtils.Assert(_currentClass.Name == node.Name);
                _currentClass = null;
                base.PostWalk(node);
            }

            public override bool Walk(FunctionDefinition node) {
                if (_currentClass == null) return false;

                if (IsSetupMethod(node)) {
                    _currentClass.SetupMethodName = node.Name;
                } else if (IsTestMethod(node)) {
                    _currentClass.TestMethodNames.Add(node.Name);
                } else if (IsTeardownMethod(node)) {
                    _currentClass.TeardownMethodName = node.Name;
                }

                return base.Walk(node);
            }

            private bool IsTeardownMethod(FunctionDefinition node) {
                return string.Equals(node.Name, _teardownName, StringComparison.InvariantCulture);
            }

            private bool IsSetupMethod(FunctionDefinition node) {
                return string.Equals(node.Name, _setupName, StringComparison.InvariantCulture);
            }

            private bool IsTestClass(ClassDefinition node) {
                return node.Name.StartsWith(_classPrefix);
            }

            private bool IsTestMethod(FunctionDefinition node) {
                return node.Name.StartsWith(_methodPrefix);
            }
        }
    }
}
