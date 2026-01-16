namespace Js2IL.Tests.Promise;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("Promise")
    {       
    }

    // Tests sorted alphabetically
    [Fact]
    public Task Promise_All_AllResolved()
    {
        return ExecutionTest(nameof(Promise_All_AllResolved));
    }

    [Fact]
    public Task Promise_All_EmptyArray()
    {
        return ExecutionTest(nameof(Promise_All_EmptyArray));
    }

    [Fact]
    public Task Promise_All_MixedValues()
    {
        return ExecutionTest(nameof(Promise_All_MixedValues));
    }

    [Fact]
    public Task Promise_All_OneRejected()
    {
        return ExecutionTest(nameof(Promise_All_OneRejected));
    }

    [Fact]
    public Task Promise_All_OrderPreserved()
    {
        return ExecutionTest(nameof(Promise_All_OrderPreserved));
    }

    [Fact]
    public  Task Promise_All_String()
    {
        return ExecutionTest(nameof(Promise_All_String));
    }

    [Fact]
    public Task Promise_All_NullIterable()
    {
        return ExecutionTest(nameof(Promise_All_NullIterable));
    }

    [Fact]
    public Task Promise_AllSettled_AllRejected()
    {
        return ExecutionTest(nameof(Promise_AllSettled_AllRejected));
    }

    [Fact]
    public Task Promise_AllSettled_AllResolved()
    {
        return ExecutionTest(nameof(Promise_AllSettled_AllResolved));
    }

    [Fact]
    public Task Promise_AllSettled_EmptyArray()
    {
        return ExecutionTest(nameof(Promise_AllSettled_EmptyArray));
    }

    [Fact]
    public Task Promise_AllSettled_MixedResults()
    {
        return ExecutionTest(nameof(Promise_AllSettled_MixedResults));
    }

    [Fact]
    public Task Promise_AllSettled_MixedValues()
    {
        return ExecutionTest(nameof(Promise_AllSettled_MixedValues));
    }

    [Fact]
    public Task Promise_AllSettled_NullIterable()
    {
        return ExecutionTest(nameof(Promise_AllSettled_NullIterable));
    }

    [Fact]
    public Task Promise_Any_AllRejected()
    {
        return ExecutionTest(nameof(Promise_Any_AllRejected));
    }

    [Fact]
    public Task Promise_Any_EmptyArray()
    {
        return ExecutionTest(nameof(Promise_Any_EmptyArray));
    }

    [Fact]
    public Task Promise_Any_FirstResolved()
    {
        return ExecutionTest(nameof(Promise_Any_FirstResolved));
    }

    [Fact]
    public Task Promise_Any_OneResolved()
    {
        return ExecutionTest(nameof(Promise_Any_OneResolved));
    }

    [Fact]
    public Task Promise_Any_NullIterable()
    {
        return ExecutionTest(nameof(Promise_Any_NullIterable));
    }

    [Fact]
    public Task Promise_Catch_ReturnsRejectedPromise()
    {
        return ExecutionTest(nameof(Promise_Catch_ReturnsRejectedPromise));
    }

    [Fact]
    public Task Promise_Catch_ReturnsResolvedPromise()
    {
        return ExecutionTest(nameof(Promise_Catch_ReturnsResolvedPromise));
    }

    [Fact]
    public Task Promise_Executor_Rejected()
    {
        return ExecutionTest(nameof(Promise_Executor_Rejected));
    }

    [Fact]
    public Task Promise_Executor_Resolved()
    {
        return ExecutionTest(nameof(Promise_Executor_Resolved));
    }

    [Fact]
    public Task Promise_Finally_ReturnsRejectedPromise()
    {
        return ExecutionTest(nameof(Promise_Finally_ReturnsRejectedPromise));
    }

    [Fact]
    public Task Promise_Finally_ReturnsResolvedPromise()
    {
        return ExecutionTest(nameof(Promise_Finally_ReturnsResolvedPromise));
    }

    [Fact]
    public Task Promise_Reject_FinallyCatch()
    {
        return ExecutionTest(nameof(Promise_Reject_FinallyCatch));
    }

    [Fact]
    public Task Promise_Reject_Then()
    {
        return ExecutionTest(nameof(Promise_Reject_Then));
    }

    [Fact]
    public Task Promise_Resolve_FinallyThen()
    {
        return ExecutionTest(nameof(Promise_Resolve_FinallyThen));
    }

    [Fact]
    public Task Promise_Resolve_FinallyThrows()
    {
        return ExecutionTest(nameof(Promise_Resolve_FinallyThrows));
    }

    [Fact]
    public Task Promise_Resolve_Then()
    {
        return ExecutionTest(nameof(Promise_Resolve_Then));
    }

    [Fact]
    public Task Promise_Resolve_ThenFinally()
    {
        return ExecutionTest(nameof(Promise_Resolve_ThenFinally));
    }

    [Fact]
    public Task Promise_Scheduling_StarvationTest()
    {
        return ExecutionTest(nameof(Promise_Scheduling_StarvationTest));
    }

    [Fact]
    public Task Promise_Then_ReturnsRejectedPromise()
    {
        return ExecutionTest(nameof(Promise_Then_ReturnsRejectedPromise));
    }

    [Fact]
    public Task Promise_Then_ReturnsResolvedPromise()
    {
        return ExecutionTest(nameof(Promise_Then_ReturnsResolvedPromise));
    }

    [Fact]
    public Task Promise_Thenable_Nested()
    {
        return ExecutionTest(nameof(Promise_Thenable_Nested));
    }

    [Fact]
    public Task Promise_Thenable_NonFunctionThen()
    {
        return ExecutionTest(nameof(Promise_Thenable_NonFunctionThen));
    }

    [Fact]
    public Task Promise_Thenable_Reject()
    {
        return ExecutionTest(nameof(Promise_Thenable_Reject));
    }

    [Fact]
    public Task Promise_Thenable_Resolve_Delayed()
    {
        return ExecutionTest(nameof(Promise_Thenable_Resolve_Delayed));
    }

    [Fact]
    public Task Promise_Thenable_Resolve_Immediate()
    {
        return ExecutionTest(nameof(Promise_Thenable_Resolve_Immediate));
    }

    [Fact]
    public Task Promise_Thenable_Returned_FromHandler()
    {
        return ExecutionTest(nameof(Promise_Thenable_Returned_FromHandler));
    }

    [Fact]
    public Task Promise_WithResolvers_Idempotent()
    {
        return ExecutionTest(nameof(Promise_WithResolvers_Idempotent));
    }

    [Fact]
    public Task Promise_WithResolvers_Reject()
    {
        return ExecutionTest(nameof(Promise_WithResolvers_Reject));
    }

    [Fact]
    public Task Promise_WithResolvers_Resolve()
    {
        return ExecutionTest(nameof(Promise_WithResolvers_Resolve));
    }

    [Fact]
    public Task Promise_Race_EmptyArray()
    {
        return ExecutionTest(nameof(Promise_Race_EmptyArray));
    }

    [Fact]
    public Task Promise_Race_FirstRejected()
    {
        return ExecutionTest(nameof(Promise_Race_FirstRejected));
    }

    [Fact]
    public Task Promise_Race_FirstResolved()
    {
        return ExecutionTest(nameof(Promise_Race_FirstResolved));
    }

    [Fact]
    public Task Promise_Race_MixedValues()
    {
        return ExecutionTest(nameof(Promise_Race_MixedValues));
    }

    [Fact]
    public Task Promise_Race_NullIterable()
    {
        return ExecutionTest(nameof(Promise_Race_NullIterable));
    }
}