// Guids.cs
// MUST match guids.h
using System;

namespace IronPythonTools.Testing
{
    static class GuidList
    {
        public const string guidIPyIntegrationVSTestingPkgString = "62c302e7-02ac-4c21-ae97-6ca1f25cbcb3";
        public const string guidIPyIntegrationVSTestingCmdSetString = "c8403743-5cc8-4276-9850-0e55762eed92";

        public static readonly Guid guidIPyIntegrationVSTestingCmdSet = new Guid(guidIPyIntegrationVSTestingCmdSetString);
    };
}