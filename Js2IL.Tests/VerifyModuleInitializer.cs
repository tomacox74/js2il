using System.Runtime.CompilerServices;
using VerifyTests;

namespace Js2IL.Tests;

public static class VerifyModuleInitializer
{
    [ModuleInitializer]
    public static void Init()
    {
        // Temporarily allow Verify to include diff content in CI logs.
        // This helps diagnose cross-platform snapshot differences.
        //VerifierSettings.OmitContentFromException();
    }
}
