using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.RegExp.dotall;

public partial class ExecutionTests
{
    [Fact(DisplayName = "with-dotall-unicode.js")]
    public Task with_dotall_unicode() => ExecutionTestFromFile("with-dotall-unicode");

    [Fact(DisplayName = "with-dotall.js")]
    public Task with_dotall() => ExecutionTestFromFile("with-dotall");

    [Fact(DisplayName = "without-dotall-unicode.js")]
    public Task without_dotall_unicode() => ExecutionTestFromFile("without-dotall-unicode");

    [Fact(DisplayName = "without-dotall.js")]
    public Task without_dotall() => ExecutionTestFromFile("without-dotall");

}
