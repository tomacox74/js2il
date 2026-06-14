using System.Runtime.CompilerServices;
using VerifyTests;

namespace Jroc.Tests;

public static class VerifyModuleInitializer
{
    [ModuleInitializer]
    public static void Init()
    {
        VerifierSettings.OmitContentFromException();
    }
}
