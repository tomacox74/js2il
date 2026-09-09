namespace Jroc.Tests;

public sealed class Test262AsyncHarnessTests
{
    [Theory]
    [InlineData("$DONE();")]
    [InlineData("$DONE(undefined);")]
    [InlineData("Promise.resolve().then(() => $DONE());")]
    [InlineData("setTimeout(() => $DONE(), 1);")]
    [InlineData("asyncTest(() => Promise.resolve());")]
    [InlineData("asyncTest(() => Promise.resolve(42));")]
    [InlineData("asyncTest(() => Promise.resolve(new Error('not a rejection')));")]
    [InlineData("asyncTest(async () => { await assert.throwsAsync(TypeError, () => Promise.reject(new TypeError())); });")]
    [InlineData("asyncTest(async () => { await assert.throwsAsync(TypeError, () => ({ then(resolve, reject) { reject(new TypeError()); } })); });")]
    [InlineData("asyncTest(async () => { function Custom() {} await assert.throwsAsync(Custom, () => Promise.reject(new Custom())); });")]
    [InlineData("asyncTest(async () => { await assert.throwsAsync(TypeError, () => Promise.reject(new TypeError())); return 42; });")]
    [InlineData("asyncTest(async () => { await assert.throwsAsync(Test262Error, () => assert.throwsAsync(TypeError, () => { throw new TypeError(); })); });")]
    public void AsyncSuccessCompletes(string source)
    {
        var result = Execute(source);
        Assert.Null(result.UnhandledException);
        Assert.Empty(result.Output);
    }

    [Theory]
    [InlineData("undefined")]
    [InlineData("null")]
    [InlineData("0")]
    [InlineData("-0")]
    [InlineData("false")]
    [InlineData("''")]
    [InlineData("NaN")]
    [InlineData("0n")]
    public void FalsyCompletionMatchesPinnedProtocol(string value)
    {
        foreach (var source in new[]
        {
            $"$DONE({value});",
            $"asyncTest(() => Promise.reject({value}));",
            $"asyncTest(() => {{ throw {value}; }});"
        })
        {
            var result = Execute(source);
            Assert.Null(result.UnhandledException);
            Assert.Empty(result.Output);
        }
    }

    [Theory]
    [InlineData("", "without calling $DONE")]
    [InlineData("new Promise(() => {});", "without calling $DONE")]
    [InlineData("asyncTest(() => new Promise(() => {}));", "without calling $DONE")]
    [InlineData("Promise.resolve().then(() => { throw new Error('lost rejection'); });", "without calling $DONE")]
    [InlineData("$DONE(new Error('done failure'));", "done failure")]
    [InlineData("$DONE(true);", "non-exception value")]
    [InlineData("$DONE(1);", "non-exception value")]
    [InlineData("$DONE('failure');", "non-exception value")]
    [InlineData("$DONE({});", "non-exception value")]
    [InlineData("Promise.resolve().then(() => $DONE(new Error('reaction failure')));", "reaction failure")]
    [InlineData("asyncTest(async () => { assert.sameValue(1, 2, 'async assertion'); });", "async assertion")]
    [InlineData("asyncTest(() => Promise.reject(new Error('rejected error')));", "rejected error")]
    [InlineData("asyncTest(() => Promise.reject(1));", "non-exception value")]
    [InlineData("asyncTest(() => { throw new Error('sync failure'); });", "sync failure")]
    [InlineData("asyncTest(() => { throw 'failure'; });", "non-exception value")]
    [InlineData("asyncTest(42);", "non-function argument")]
    [InlineData("$DONE(); $DONE();", "more than once")]
    [InlineData("$DONE(null); $DONE(false);", "more than once")]
    [InlineData("$DONE(new Error('first failure')); $DONE();", "first failure")]
    [InlineData("$DONE(); Promise.resolve().then(() => $DONE(new Error('late failure')));", "late failure")]
    [InlineData("asyncTest(async () => { await assert.throwsAsync(TypeError, () => { throw new TypeError(); }); });", "function threw synchronously")]
    [InlineData("asyncTest(async () => { await assert.throwsAsync(TypeError, () => Promise.resolve()); });", "no exception was thrown")]
    [InlineData("asyncTest(async () => { await assert.throwsAsync(TypeError, () => Promise.reject(new RangeError())); });", "got a RangeError")]
    [InlineData("asyncTest(async () => { await assert.throwsAsync(TypeError, () => Promise.reject(undefined)); });", "not an object")]
    [InlineData("asyncTest(async () => { await assert.throwsAsync(TypeError, () => Promise.reject(null)); });", "not an object")]
    [InlineData("asyncTest(async () => { await assert.throwsAsync(TypeError, () => Promise.reject(42)); });", "not an object")]
    [InlineData("asyncTest(async () => { await assert.throwsAsync(TypeError, () => null); });", "not a thenable")]
    [InlineData("asyncTest(async () => { await assert.throwsAsync(TypeError, () => ({})); });", "not a thenable")]
    [InlineData("asyncTest(async () => { await assert.throwsAsync(null, () => Promise.reject()); });", "not an error constructor")]
    [InlineData("asyncTest(async () => { await assert.throwsAsync(TypeError, null); });", "not a function")]
    [InlineData("asyncTest(async () => { await assert.throwsAsync(TypeError, () => ({ then() { throw new TypeError(); } })); });", ".then threw synchronously")]
    [InlineData("asyncTest(async () => { await assert.throwsAsync(TypeError, () => ({ then(resolve, reject) { reject(new TypeError()); throw new Error(); } })); });", ".then threw synchronously")]
    [InlineData("asyncTest(async () => { const Other = function TypeError() {}; await assert.throwsAsync(TypeError, () => Promise.reject(new Other())); });", "different error constructor with the same name")]
    public void AsyncFailureIsNotSwallowed(string source, string expectedMessage)
    {
        var exception = Record.Exception(() => Execute(source));
        Assert.NotNull(exception);
        Assert.Contains(expectedMessage, exception.ToString());
    }

    [Fact]
    public void SynchronousTestsDoNotRequireDone()
        => Assert.Empty(Execute("assert.sameValue(1, 1);", flags: "").Output);

    [Fact]
    public void AsyncTestRequiresAsyncFlag()
    {
        var exception = Record.Exception(() => Execute("asyncTest(() => Promise.resolve());", flags: ""));
        Assert.NotNull(exception);
        Assert.Contains("without async flag", exception.ToString());
    }

    [Theory]
    [InlineData("  - async")]
    [InlineData("- async")]
    public void BlockStyleAsyncMetadataRequiresCompletion(string flag)
    {
        var exception = Assert.Throws<InvalidOperationException>(() =>
            Test262SharedAssertHarness.CompileAndExecute(
                "missing_done", "Test262AsyncHarness",
                _ => ($"/*---\nflags:\n{flag}\n---*/",
                    Path.Combine(Directory.GetCurrentDirectory(), "missing_done.js"))));
        Assert.Contains("without calling $DONE", exception.Message);
    }

    [Fact]
    public void BlockStyleIncludesRegistersAsyncHelpers()
    {
        var result = Test262SharedAssertHarness.CompileAndExecute(
            "async_helpers", "Test262AsyncHarness",
            _ => ("""
                /*---
                flags:
                  - async
                includes:
                  - asyncHelpers.js
                ---*/
                asyncTest(async () => {
                  await assert.throwsAsync(TypeError, () => Promise.reject(new TypeError()));
                });
                """, Path.Combine(Directory.GetCurrentDirectory(), "async_helpers.js")));
        Assert.Null(result.UnhandledException);
    }

    [Fact]
    public void ThrowsAsyncReturnsRejectedPromiseInsteadOfThrowingSynchronously()
        => Execute("""
            const result = assert.throwsAsync(TypeError, () => { throw new TypeError(); });
            assert.sameValue(typeof result.then, "function");
            result.then(() => $DONE(new Error("must reject")), error => {
              assert.sameValue(error.name, "Test262Error");
              $DONE();
            });
            """);

    [Fact]
    public void AsyncRuntimeNegativeUsesRecordedFailure()
    {
        var result = Test262SharedAssertHarness.CompileAndExecute(
            "negative", "Test262AsyncHarness",
            _ => ("""
                /*---
                flags: [async]
                includes: [asyncHelpers.js]
                negative:
                  phase: runtime
                  type: TypeError
                ---*/
                asyncTest(() => Promise.reject(new TypeError("expected rejection")));
                """, Path.Combine(Directory.GetCurrentDirectory(), "negative.js")),
            allowUnhandledException: true);
        Assert.IsType<JavaScriptRuntime.TypeError>(result.UnhandledException);
    }

    [Fact]
    public void CompletionAndFailureDoNotLeakBetweenInvocations()
    {
        Execute("$DONE();");
        Assert.Throws<InvalidOperationException>(() => Execute(""));
        Assert.NotNull(Record.Exception(() => Execute("$DONE(new Error('failure'));")));
        Execute("$DONE();");
    }

    [Fact]
    public async Task ParallelExecutionsHaveIndependentCompletion()
    {
        await Task.WhenAll(
            Task.Run(() => Execute("Promise.resolve().then(() => $DONE());")),
            Task.Run(() => Assert.NotNull(Record.Exception(() => Execute("asyncTest(() => Promise.reject(new Error('failure')));")))),
            Task.Run(() => Assert.Throws<InvalidOperationException>(() => Execute(""))));
    }

    private static InMemoryTestExecutionResult Execute(string source, string flags = "async")
        => Test262SharedAssertHarness.CompileAndExecute(
            "async_harness", "Test262AsyncHarness",
            _ => ($"/*---\nflags: [{flags}]\nincludes: [asyncHelpers.js]\n---*/\n{source}",
                Path.Combine(Directory.GetCurrentDirectory(), "async_harness.js")));
}
