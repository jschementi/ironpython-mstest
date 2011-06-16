
/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.VisualStudio.Shell;
using Microsoft.Win32;
using VSLangProj80;
using System.Collections.Generic;


namespace IronPythonTools.Testing
{
    /// <summary>
    /// This is the generator class. 
    /// When setting the 'Custom Tool' property of a C# project item to "PythonUnittestIntegration", 
    /// the GenerateCode function will get called and will return the contents of the generated file 
    /// to the project system
    /// </summary>
    [ComVisible(true)]
    [Guid("52B316AA-1997-4c81-9969-83604C09EEB4")]
    [CodeGeneratorRegistration(typeof(IronPythonTestingGenerator), "IronPython unit testing in Visual Studio", vsContextGuids.vsContextGuidVCSProject, GeneratesDesignTimeSource = true)]
    [CodeGeneratorRegistration(typeof(IronPythonTestingGenerator), "IronPython unit testing in Visual Studio", vsContextGuids.vsContextGuidVBProject, GeneratesDesignTimeSource = true)]
    [ProvideObject(typeof(IronPythonTestingGenerator))]
    public class IronPythonTestingGenerator : BaseCodeGeneratorWithSite
    {
#pragma warning disable 0414
        //The name of this generator (use for 'Custom Tool' property of project item)
        internal static string name = "IronPythonTestingGenerator";
#pragma warning restore 0414

        internal static bool valid;

        internal IronPythonTestingParser pythonTestingParser;

        /// <summary>
        /// Function that builds the contents of the generated file based on the contents of the input file
        /// </summary>
        /// <param name="inputFileContent">Content of the input file</param>
        /// <returns>Generated file as a byte array</returns>
        protected override byte[] GenerateCode(string inputFileContent)
        {
            CodeDomProvider provider = GetCodeProvider();
            
            try
            {
                var testingParser = new IronPythonTestingParser(this.InputFilePath, inputFileContent);

                var projectFileName = this.GetProject().FullName;

                var dte = ((EnvDTE._DTE) this.GetService(typeof (EnvDTE._DTE)));

                if (projectFileName == null) return null;

                var configPath = Path.Combine(Path.GetDirectoryName(projectFileName) ?? string.Empty, "__config__.py");
                var deploymentItems = new List<List<string>>();
                if (File.Exists(configPath))
                {
                    deploymentItems = testingParser.GetDeploymentItems(
                        configPath,
                        File.ReadAllText(configPath));
                }

                var generatedCode = MsTestGenerator.Generate(
                    this.InputFilePath,
                    projectFileName,
                    testingParser.GetStructure(),
                    FileNameSpace,
                    deploymentItems);

                if (this.CodeGeneratorProgress != null)
                {
                    //Report that we are 1/2 done
                    this.CodeGeneratorProgress.Progress(50, 100);
                }

                using (StringWriter writer = new StringWriter(new StringBuilder()))
                {
                    writer.Write(generatedCode);

                    if (this.CodeGeneratorProgress != null)
                    {
                        //Report that we are done
                        this.CodeGeneratorProgress.Progress(100, 100);
                    }
                    writer.Flush();

                    //Get the Encoding used by the writer. We're getting the WindowsCodePage encoding, 
                    //which may not work with all languages
                    Encoding enc = Encoding.GetEncoding(writer.Encoding.WindowsCodePage);
                    
                    //Get the preamble (byte-order mark) for our encoding
                    byte[] preamble = enc.GetPreamble();
                    int preambleLength = preamble.Length;
                    
                    //Convert the writer contents to a byte array
                    byte[] body = enc.GetBytes(writer.ToString());

                    //Prepend the preamble to body (store result in resized preamble array)
                    Array.Resize<byte>(ref preamble, preambleLength + body.Length);
                    Array.Copy(body, 0, preamble, preambleLength, body.Length);
                    
                    //Return the combined byte array
                    return preamble;                    
                }
            }
            catch (Exception e)
            {
                this.GeneratorError(4, e.ToString(), 1, 1);
                //Returning null signifies that generation has failed
                return null;
            }
        }

        /// <summary>
        /// Receives any errors that occur while validating the documents's schema.
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="args">Details about the validation error that has occurred</param>
        private void OnValidationError(object sender, dynamic args)
        {
            //signal that validation of document against schema has failed
            valid = false;

            //Report the error (so that it is shown in the error list)
            this.GeneratorError(4, args.Exception.Message, (uint)args.Exception.LineNumber - 1, (uint)args.Exception.LinePosition - 1);
        }
    }
}
