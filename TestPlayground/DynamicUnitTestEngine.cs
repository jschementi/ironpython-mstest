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
using Microsoft.Scripting.Hosting;
using IronPython.Hosting;
using System.IO;
using System.Reflection;

#if SILVERLIGHT
using Microsoft.Scripting.Silverlight;
#endif

namespace TestPlayground
{
    public class DynamicUnitTestEngine
    {
        private ScriptEngine engine;

        private ScriptScope module;

        private object klass;

        private object klassInstance;

        public static DynamicUnitTestEngine Create(string testClassName, string testFileName) {
            var self = new DynamicUnitTestEngine();
            self.Init(testClassName, testFileName);
            return self;
        }

        private void Init(string testClassName, string testFileName) {
            InitializeDLR();
            ExecuteTestFile(testFileName);
            ConstructClass(testClassName);
        }

        private void ExecuteTestFile(string file) {
#if SILVERLIGHT
			var filepath = file;
#else
            var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var filepath = Path.Combine(assemblyPath, file);
#endif
            module = engine.ExecuteFile(filepath);
        }

        private void ConstructClass(string testClassName) {
            klass = engine.Operations.GetMember(module, testClassName);
            klassInstance = ((dynamic)klass)();
        }

        public void InvokeTestMethod(string testMethodName) {
            InvokeDynamicMember(testMethodName);
        }

        public void InvokeSetup() {
            InvokeDynamicMember("setUp", fail: false);
        }

        public void InvokeTeardown() {
            InvokeDynamicMember("tearDown", fail: false);
        }

        private dynamic InvokeDynamicMember(string methodName, object scope = null, bool fail = true) {
            dynamic method = null;
            if (fail) {
                method = engine.Operations.GetMember(klassInstance ?? scope, methodName);
                return method();
            } else {
                if (engine.Operations.TryGetMember(klassInstance ?? scope, methodName, out method)) {
                    return method();
                }
            }
            return null;
        }

        private void InitializeDLR() {
#if SILVERLIGHT
            var setup = DynamicApplication.CreateRuntimeSetup();
#else
            var setup = Python.CreateRuntimeSetup(new Dictionary<string, object>());
#endif
            setup.DebugMode = System.Diagnostics.Debugger.IsAttached;

            var runtime = new ScriptRuntime(setup);
            engine = Python.GetEngine(runtime);
        }
    }
}
