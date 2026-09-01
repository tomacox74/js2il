using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Date.prototype.toJSON;

public partial class ExecutionTests
{
    [Fact(DisplayName = "called-as-function.js")]
    public Task called_as_function() => ExecutionTestFromFile("called-as-function");

    [Fact(DisplayName = "length.js")]
    public Task length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name.js")]
    public Task name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "not-a-constructor.js")]
    public Task not_a_constructor() => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "to-primitive-value-of.js")]
    public Task to_primitive_value_of() => ExecutionTestFromFile("to-primitive-value-of");

}
