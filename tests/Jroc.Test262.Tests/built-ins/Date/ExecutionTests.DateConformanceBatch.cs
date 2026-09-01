using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Date;

public partial class ExecutionTests
{
    [Fact(DisplayName = "is-a-constructor.js")]
    public Task is_a_constructor() => ExecutionTestFromFile("is-a-constructor");

    [Fact(DisplayName = "prop-desc.js")]
    public Task prop_desc() => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "subclassing.js")]
    public Task subclassing() => ExecutionTestFromFile("subclassing");

    [Fact(DisplayName = "value-get-symbol-to-prim-err.js")]
    public Task value_get_symbol_to_prim_err() => ExecutionTestFromFile("value-get-symbol-to-prim-err");

    [Fact(DisplayName = "value-symbol-to-prim-err.js")]
    public Task value_symbol_to_prim_err() => ExecutionTestFromFile("value-symbol-to-prim-err");

    [Fact(DisplayName = "value-symbol-to-prim-return-obj.js")]
    public Task value_symbol_to_prim_return_obj() => ExecutionTestFromFile("value-symbol-to-prim-return-obj");

    [Fact(DisplayName = "value-to-primitive-call-err.js")]
    public Task value_to_primitive_call_err() => ExecutionTestFromFile("value-to-primitive-call-err");

    [Fact(DisplayName = "value-to-primitive-get-meth-err.js")]
    public Task value_to_primitive_get_meth_err() => ExecutionTestFromFile("value-to-primitive-get-meth-err");

    [Fact(DisplayName = "value-to-primitive-result-faulty.js")]
    public Task value_to_primitive_result_faulty() => ExecutionTestFromFile("value-to-primitive-result-faulty");

    [Fact(DisplayName = "value-to-primitive-result-non-string-prim.js")]
    public Task value_to_primitive_result_non_string_prim() => ExecutionTestFromFile("value-to-primitive-result-non-string-prim");

}
