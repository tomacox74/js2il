using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Set.prototype.values;

public class SetConformanceBatchExecutionTests : DiskExecutionTestsBase
{
    public SetConformanceBatchExecutionTests() : base("built_ins.Set.prototype.values") { }

    [Fact(DisplayName = "not-a-constructor.js")]
    public Task not_a_constructor() => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "returns-iterator.js")]
    public Task returns_iterator() => ExecutionTestFromFile("returns-iterator");

    [Fact(DisplayName = "this-not-object-throw-boolean.js")]
    public Task this_not_object_throw_boolean() => ExecutionTestFromFile("this-not-object-throw-boolean");

    [Fact(DisplayName = "this-not-object-throw-null.js")]
    public Task this_not_object_throw_null() => ExecutionTestFromFile("this-not-object-throw-null");

    [Fact(DisplayName = "this-not-object-throw-number.js")]
    public Task this_not_object_throw_number() => ExecutionTestFromFile("this-not-object-throw-number");

    [Fact(DisplayName = "this-not-object-throw-string.js")]
    public Task this_not_object_throw_string() => ExecutionTestFromFile("this-not-object-throw-string");

    [Fact(DisplayName = "this-not-object-throw-symbol.js")]
    public Task this_not_object_throw_symbol() => ExecutionTestFromFile("this-not-object-throw-symbol");

    [Fact(DisplayName = "this-not-object-throw-undefined.js")]
    public Task this_not_object_throw_undefined() => ExecutionTestFromFile("this-not-object-throw-undefined");

}
