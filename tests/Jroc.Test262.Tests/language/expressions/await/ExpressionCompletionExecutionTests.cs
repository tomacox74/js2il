using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.expressions.await;

public sealed class ExpressionCompletionExecutionTests : DiskExecutionTestsBase
{
    public ExpressionCompletionExecutionTests() : base("language.expressions.await") { }

    [Fact(DisplayName = "async-await-interleaved.js")]
    public Task test_async_await_interleaved() => ExecutionTest("async-await-interleaved");

    [Fact(DisplayName = "async-generator-interleaved.js")]
    public Task test_async_generator_interleaved() => ExecutionTest("async-generator-interleaved");

    [Fact(DisplayName = "await-awaits-thenable-not-callable.js")]
    public Task test_await_awaits_thenable_not_callable() => ExecutionTest("await-awaits-thenable-not-callable");

    [Fact(DisplayName = "await-awaits-thenables-that-throw.js")]
    public Task test_await_awaits_thenables_that_throw() => ExecutionTest("await-awaits-thenables-that-throw");

    [Fact(DisplayName = "await-awaits-thenables.js")]
    public Task test_await_awaits_thenables() => ExecutionTest("await-awaits-thenables");

    [Fact(DisplayName = "await-monkey-patched-promise.js")]
    public Task test_await_monkey_patched_promise() => ExecutionTest("await-monkey-patched-promise");

    [Fact(DisplayName = "await-non-promise-thenable.js")]
    public Task test_await_non_promise_thenable() => ExecutionTest("await-non-promise-thenable");

    [Fact(DisplayName = "await-non-promise.js")]
    public Task test_await_non_promise() => ExecutionTest("await-non-promise");

    [Fact(DisplayName = "await-throws-rejections.js")]
    public Task test_await_throws_rejections() => ExecutionTest("await-throws-rejections");

    [Fact(DisplayName = "for-await-of-interleaved.js")]
    public Task test_for_await_of_interleaved() => ExecutionTest("for-await-of-interleaved");

    [Fact(DisplayName = "syntax-await-has-UnaryExpression-with-MultiplicativeExpression.js")]
    public Task test_syntax_await_has_UnaryExpression_with_MultiplicativeExpression() => ExecutionTest("syntax-await-has-UnaryExpression-with-MultiplicativeExpression");

    [Fact(DisplayName = "syntax-await-has-UnaryExpression.js")]
    public Task test_syntax_await_has_UnaryExpression() => ExecutionTest("syntax-await-has-UnaryExpression");

    [Fact(DisplayName = "syntax-await-in-ConditionalExpression.js")]
    public Task test_syntax_await_in_ConditionalExpression() => ExecutionTest("syntax-await-in-ConditionalExpression");
}
