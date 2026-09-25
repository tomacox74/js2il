using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.RegExp.prototype.flags;

public class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.RegExp.prototype.flags") { }

    [Fact(DisplayName = "length.js")]
    public Task length()
        => ExecutionTestFromFile("length");

    [Fact(DisplayName = "prop-desc.js")]
    public Task prop_desc()
        => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "return-order.js")]
    public Task return_order()
        => ExecutionTestFromFile("return-order");

    [Fact(DisplayName = "this-val-non-obj.js")]
    public Task this_val_non_obj()
        => ExecutionTestFromFile("this-val-non-obj");

    [Fact(DisplayName = "coercion-global.js")]
    public Task coercion_global() => ExecutionTestFromFile("coercion-global");

    [Fact(DisplayName = "coercion-multiline.js")]
    public Task coercion_multiline() => ExecutionTestFromFile("coercion-multiline");

    [Fact(DisplayName = "coercion-dotall.js")]
    public Task coercion_dotall() => ExecutionTestFromFile("coercion-dotall");

    [Fact(DisplayName = "coercion-hasIndices.js")]
    public Task coercion_hasIndices() => ExecutionTestFromFile("coercion-hasIndices");

    [Fact(DisplayName = "coercion-ignoreCase.js")]
    public Task coercion_ignoreCase() => ExecutionTestFromFile("coercion-ignoreCase");

    [Fact(DisplayName = "coercion-sticky.js")]
    public Task coercion_sticky() => ExecutionTestFromFile("coercion-sticky");

    [Fact(DisplayName = "coercion-unicode.js")]
    public Task coercion_unicode() => ExecutionTestFromFile("coercion-unicode");

    [Fact(DisplayName = "get-order.js")]
    public Task get_order() => ExecutionTestFromFile("get-order");

    [Fact(DisplayName = "rethrow.js")]
    public Task rethrow() => ExecutionTestFromFile("rethrow");

    [Fact(DisplayName = "this-val-regexp-prototype.js")]
    public Task this_val_regexp_prototype() => ExecutionTestFromFile("this-val-regexp-prototype");
}
