using System.Runtime.CompilerServices;
using VerifyTests;

namespace Js2IL.Tests;

public static class VerifyModuleInitializer
{
    [ModuleInitializer]
    public static void Init()
    {
        VerifierSettings.OmitContentFromException();
    }
}
