using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Symbol.prototype.description;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Symbol.prototype.description") { }

    [Fact(DisplayName = "description-symboldescriptivestring")]
    public Task description_symboldescriptivestring()
        => ExecutionTestFromFile("description-symboldescriptivestring");

    [Fact(DisplayName = "get")]
    public Task get()
        => ExecutionTestFromFile("get");

    [Fact(DisplayName = "descriptor")]
    public Task descriptor()
        => ExecutionTestFromFile("descriptor");

    [Fact(DisplayName = "this-val-symbol")]
    public Task this_val_symbol()
        => ExecutionTestFromFile("this-val-symbol");

    [Fact(DisplayName = "this-val-non-symbol")]
    public Task this_val_non_symbol()
        => ExecutionTestFromFile("this-val-non-symbol");

    [Fact(DisplayName = "wrapper")]
    public Task wrapper()
        => ExecutionTestFromFile("wrapper");

    [Fact(DisplayName = "is-not-own-property")]
    public Task is_not_own_property()
        => ExecutionTestFromFile("is-not-own-property");
}
