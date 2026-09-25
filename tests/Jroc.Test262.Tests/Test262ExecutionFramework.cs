using System.Reflection;
using Xunit.Abstractions;
using Xunit.Sdk;

[assembly: TestFramework("Jroc.Test262.Tests.Test262ExecutionFramework", "Jroc.Test262.Tests")]

namespace Jroc.Test262.Tests;

public sealed class Test262ExecutionFramework(IMessageSink messageSink) : XunitTestFramework(messageSink)
{
    protected override ITestFrameworkExecutor CreateExecutor(AssemblyName assemblyName)
        => new FolderAssemblyExecutor(assemblyName, SourceInformationProvider, DiagnosticMessageSink);

    private sealed class FolderAssemblyExecutor(
        AssemblyName assemblyName,
        ISourceInformationProvider sourceProvider,
        IMessageSink diagnosticSink)
        : XunitTestFrameworkExecutor(assemblyName, sourceProvider, diagnosticSink)
    {
        protected override async void RunTestCases(
            IEnumerable<IXunitTestCase> testCases,
            IMessageSink executionMessageSink,
            ITestFrameworkExecutionOptions executionOptions)
        {
            using var cache = new Test262FolderAssemblyCache();
            Test262FolderAssemblyCache.Active = cache;
            try
            {
                using var runner = new XunitTestAssemblyRunner(
                    TestAssembly, testCases, DiagnosticMessageSink, executionMessageSink, executionOptions);
                await runner.RunAsync();
            }
            finally
            {
                Test262FolderAssemblyCache.Active = null;
            }
        }
    }
}
