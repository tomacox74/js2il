using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Symbol;

public class PortNext100ExecutionTests : DiskExecutionTestsBase
{
    public PortNext100ExecutionTests() : base("built_ins.Symbol") { }

    [Fact(DisplayName = "for/create-value")]
    public Task for_create_value()
        => ExecutionTestFromFile("for/create-value");

    [Fact(DisplayName = "for/description")]
    public Task for_description()
        => ExecutionTestFromFile("for/description");

    [Fact(DisplayName = "for/length")]
    public Task for_length()
        => ExecutionTestFromFile("for/length");

    [Fact(DisplayName = "for/name")]
    public Task for_name()
        => ExecutionTestFromFile("for/name");

    [Fact(DisplayName = "for/not-a-constructor")]
    public Task for_not_a_constructor()
        => ExecutionTestFromFile("for/not-a-constructor");

    [Fact(DisplayName = "for/prop-desc")]
    public Task for_prop_desc()
        => ExecutionTestFromFile("for/prop-desc");

    [Fact(DisplayName = "for/retrieve-value")]
    public Task for_retrieve_value()
        => ExecutionTestFromFile("for/retrieve-value");

    [Fact(DisplayName = "for/to-string-err")]
    public Task for_to_string_err()
        => ExecutionTestFromFile("for/to-string-err");

    [Fact(DisplayName = "keyFor/arg-non-symbol")]
    public Task keyFor_arg_non_symbol()
        => ExecutionTestFromFile("keyFor/arg-non-symbol");

    [Fact(DisplayName = "keyFor/arg-symbol-registry-hit")]
    public Task keyFor_arg_symbol_registry_hit()
        => ExecutionTestFromFile("keyFor/arg-symbol-registry-hit");

    [Fact(DisplayName = "keyFor/arg-symbol-registry-miss")]
    public Task keyFor_arg_symbol_registry_miss()
        => ExecutionTestFromFile("keyFor/arg-symbol-registry-miss");

    [Fact(DisplayName = "keyFor/length")]
    public Task keyFor_length()
        => ExecutionTestFromFile("keyFor/length");

    [Fact(DisplayName = "keyFor/name")]
    public Task keyFor_name()
        => ExecutionTestFromFile("keyFor/name");

    [Fact(DisplayName = "keyFor/not-a-constructor")]
    public Task keyFor_not_a_constructor()
        => ExecutionTestFromFile("keyFor/not-a-constructor");

    [Fact(DisplayName = "keyFor/prop-desc")]
    public Task keyFor_prop_desc()
        => ExecutionTestFromFile("keyFor/prop-desc");

}
