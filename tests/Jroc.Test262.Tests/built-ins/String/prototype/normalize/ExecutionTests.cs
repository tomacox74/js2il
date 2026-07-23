using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.String.prototype.normalize;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.String.prototype.normalize") { }

    [Fact(DisplayName = "form-is-not-valid-throws")]
    public Task form_is_not_valid_throws()
        => ExecutionTestFromFile("form-is-not-valid-throws");

    [Fact(DisplayName = "length")]
    public Task length()
        => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name")]
    public Task name()
        => ExecutionTestFromFile("name");

    [Fact(DisplayName = "normalize")]
    public Task normalize()
        => ExecutionTestFromFile("normalize");

    [Fact(DisplayName = "not-a-constructor")]
    public Task not_a_constructor()
        => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "return-abrupt-from-form-as-symbol")]
    public Task return_abrupt_from_form_as_symbol()
        => ExecutionTestFromFile("return-abrupt-from-form-as-symbol");

    [Fact(DisplayName = "return-abrupt-from-form")]
    public Task return_abrupt_from_form()
        => ExecutionTestFromFile("return-abrupt-from-form");

    [Fact(DisplayName = "return-normalized-string-from-coerced-form")]
    public Task return_normalized_string_from_coerced_form()
        => ExecutionTestFromFile("return-normalized-string-from-coerced-form");

    [Fact(DisplayName = "return-normalized-string-using-default-parameter")]
    public Task return_normalized_string_using_default_parameter()
        => ExecutionTestFromFile("return-normalized-string-using-default-parameter");

    [Fact(DisplayName = "return-normalized-string")]
    public Task return_normalized_string()
        => ExecutionTestFromFile("return-normalized-string");
}
