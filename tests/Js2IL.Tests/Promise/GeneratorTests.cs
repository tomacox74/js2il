using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Js2IL.Tests.Promise;

public class GeneratorTests : GeneratorTestsBase
{
    public GeneratorTests() : base("Promise")
    {
    }

    protected Task GenerateTest(
        string testName,
        Action<VerifySettings>? configureSettings = null,
        string[]? additionalScripts = null,
        [CallerFilePath] string sourceFilePath = "")
        => base.GenerateTest(
            testName,
            configureSettings,
            additionalScripts,
            sourceFilePath);

    [Fact]
    public Task Promise_All_AllResolved()
    {
        return GenerateTest(nameof(Promise_All_AllResolved));
    }

    [Fact]
    public Task Promise_All_EmptyArray()
    {
        return GenerateTest(nameof(Promise_All_EmptyArray));
    }

    [Fact]
    public Task Promise_All_MixedValues()
    {
        return GenerateTest(nameof(Promise_All_MixedValues));
    }

    [Fact]
    public Task Promise_All_OneRejected()
    {
        return GenerateTest(nameof(Promise_All_OneRejected));
    }

    [Fact]
    public Task Promise_All_OrderPreserved()
    {
        return GenerateTest(nameof(Promise_All_OrderPreserved));
    }

    [Fact]
    public Task Promise_All_NullIterable()
    {
        return GenerateTest(nameof(Promise_All_NullIterable));
    }

    [Fact]
    public Task Promise_AllSettled_AllRejected()
    {
        return GenerateTest(nameof(Promise_AllSettled_AllRejected));
    }

    [Fact]
    public Task Promise_AllSettled_AllResolved()
    {
        return GenerateTest(nameof(Promise_AllSettled_AllResolved));
    }

    [Fact]
    public Task Promise_AllSettled_EmptyArray()
    {
        return GenerateTest(nameof(Promise_AllSettled_EmptyArray));
    }

    [Fact]
    public Task Promise_AllSettled_MixedResults()
    {
        return GenerateTest(nameof(Promise_AllSettled_MixedResults));
    }

    [Fact]
    public Task Promise_AllSettled_MixedValues()
    {
        return GenerateTest(nameof(Promise_AllSettled_MixedValues));
    }

    [Fact]
    public Task Promise_AllSettled_NullIterable()
    {
        return GenerateTest(nameof(Promise_AllSettled_NullIterable));
    }

    [Fact]
    public Task Promise_Any_AllRejected()
    {
        return GenerateTest(nameof(Promise_Any_AllRejected));
    }

    [Fact]
    public Task Promise_Any_EmptyArray()
    {
        return GenerateTest(nameof(Promise_Any_EmptyArray));
    }

    [Fact]
    public Task Promise_Any_FirstResolved()
    {
        return GenerateTest(nameof(Promise_Any_FirstResolved));
    }

    [Fact]
    public Task Promise_Any_OneResolved()
    {
        return GenerateTest(nameof(Promise_Any_OneResolved));
    }

    [Fact]
    public Task Promise_Any_NullIterable()
    {
        return GenerateTest(nameof(Promise_Any_NullIterable));
    }

    [Fact]
    public Task Promise_Executor_Resolved()
    {
        return GenerateTest(nameof(Promise_Executor_Resolved));
    }
    [Fact]
    public Task Promise_Catch_ReturnsResolvedPromise()
    {
        return GenerateTest(nameof(Promise_Catch_ReturnsResolvedPromise));
    }

    [Fact]
    public Task Promise_Catch_ReturnsRejectedPromise()
    {
        return GenerateTest(nameof(Promise_Catch_ReturnsRejectedPromise));
    }


    [Fact]
    public Task Promise_Executor_Rejected()
    {
        return GenerateTest(nameof(Promise_Executor_Rejected));
    }

    [Fact]
    public Task Promise_Finally_ReturnsResolvedPromise()
    {
        return GenerateTest(nameof(Promise_Finally_ReturnsResolvedPromise));
    }

    [Fact]
    public Task Promise_Finally_ReturnsResolvedPromise_PassThrough_Rejected()
    {
        return GenerateTest(nameof(Promise_Finally_ReturnsResolvedPromise_PassThrough_Rejected));
    }

    [Fact]
    public Task Promise_Finally_ReturnsRejectedPromise()
    {
        return GenerateTest(nameof(Promise_Finally_ReturnsRejectedPromise));
    }

    [Fact]
    public Task Promise_Finally_ReturnsThenable_PassThrough_Fulfilled()
    {
        return GenerateTest(nameof(Promise_Finally_ReturnsThenable_PassThrough_Fulfilled));
    }

    [Fact]
    public Task Promise_Finally_ReturnsThenable_PassThrough_Rejected()
    {
        return GenerateTest(nameof(Promise_Finally_ReturnsThenable_PassThrough_Rejected));
    }

    [Fact]
    public Task Promise_Reject_FinallyCatch()
    {
        return GenerateTest(nameof(Promise_Reject_FinallyCatch));
    }

    [Fact]
    public Task Promise_Reject_Then()
    {
        return GenerateTest(nameof(Promise_Reject_Then));
    }

    [Fact]
    public Task Promise_Resolve_FinallyThen()
    {
        return GenerateTest(nameof(Promise_Resolve_FinallyThen));
    }

    [Fact]
    public Task Promise_Resolve_FinallyThrows()
    {
        return GenerateTest(nameof(Promise_Resolve_FinallyThrows));
    }

    [Fact]
    public Task Promise_Resolve_Then()
    {
        return GenerateTest(nameof(Promise_Resolve_Then));
    }

    [Fact]
    public Task Promise_Resolve_ThenFinally()
    {
        return GenerateTest(nameof(Promise_Resolve_ThenFinally));
    }

    [Fact]
    public Task Promise_Scheduling_StarvationTest()
    {
        return GenerateTest(nameof(Promise_Scheduling_StarvationTest));
    }

    [Fact]
    public Task Promise_Then_ReturnsResolvedPromise()
    {
        return GenerateTest(nameof(Promise_Then_ReturnsResolvedPromise));
    }

    [Fact]
    public Task Promise_Then_ReturnsRejectedPromise()
    {
        return GenerateTest(nameof(Promise_Then_ReturnsRejectedPromise));
    }

    [Fact]
    public Task Promise_Thenable_Nested()
    {
        return GenerateTest(nameof(Promise_Thenable_Nested));
    }

    [Fact]
    public Task Promise_Thenable_NonFunctionThen()
    {
        return GenerateTest(nameof(Promise_Thenable_NonFunctionThen));
    }

    [Fact]
    public Task Promise_Thenable_Reject()
    {
        return GenerateTest(nameof(Promise_Thenable_Reject));
    }

    [Fact]
    public Task Promise_Thenable_Resolve_Delayed()
    {
        return GenerateTest(nameof(Promise_Thenable_Resolve_Delayed));
    }

    [Fact]
    public Task Promise_Thenable_Resolve_Immediate()
    {
        return GenerateTest(nameof(Promise_Thenable_Resolve_Immediate));
    }

    [Fact]
    public Task Promise_Thenable_Returned_FromHandler()
    {
        return GenerateTest(nameof(Promise_Thenable_Returned_FromHandler));
    }

    [Fact]
    public Task Promise_WithResolvers_Idempotent()
    {
        return GenerateTest(nameof(Promise_WithResolvers_Idempotent));
    }

    [Fact]
    public Task Promise_WithResolvers_Reject()
    {
        return GenerateTest(nameof(Promise_WithResolvers_Reject));
    }

    [Fact]
    public Task Promise_WithResolvers_Resolve()
    {
        return GenerateTest(nameof(Promise_WithResolvers_Resolve));
    }

    [Fact]
    public Task Promise_Race_EmptyArray()
    {
        return GenerateTest(nameof(Promise_Race_EmptyArray));
    }

    [Fact]
    public Task Promise_Race_FirstRejected()
    {
        return GenerateTest(nameof(Promise_Race_FirstRejected));
    }

    [Fact]
    public Task Promise_Race_FirstResolved()
    {
        return GenerateTest(nameof(Promise_Race_FirstResolved));
    }

    [Fact]
    public Task Promise_Race_MixedValues()
    {
        return GenerateTest(nameof(Promise_Race_MixedValues));
    }

    [Fact]
    public Task Promise_Race_NullIterable()
    {
        return GenerateTest(nameof(Promise_Race_NullIterable));
    }
}