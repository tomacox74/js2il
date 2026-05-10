using Js2IL.Tests;

namespace Js2IL.Test262.Tests.language.expressions.class_;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("language.expressions.class_") { }

    [Fact(DisplayName = "accessor-name-inst-computed-in")]
    public Task accessor_name_inst_computed_in()
        => ExecutionTest("accessor-name-inst-computed-in");

    [Fact(DisplayName = "accessor-name-inst-computed-yield-expr")]
    public Task accessor_name_inst_computed_yield_expr()
        => ExecutionTest("accessor-name-inst-computed-yield-expr");

    [Fact(DisplayName = "accessor-name-static-computed-in")]
    public Task accessor_name_static_computed_in()
        => ExecutionTest("accessor-name-static-computed-in");

    [Fact(DisplayName = "accessor-name-static-computed-yield-expr")]
    public Task accessor_name_static_computed_yield_expr()
        => ExecutionTest("accessor-name-static-computed-yield-expr");

    [Fact(DisplayName = "class-name-ident-await-escaped")]
    public Task class_name_ident_await_escaped()
        => ExecutionTest("class-name-ident-await-escaped");

    [Fact(DisplayName = "class-name-ident-await")]
    public Task class_name_ident_await()
        => ExecutionTest("class-name-ident-await");

    [Fact(DisplayName = "constructor-this-tdz-during-initializers", Skip = "Tracked by issue #1055: derived-constructor this TDZ during field initializers is incomplete.")]
    public Task constructor_this_tdz_during_initializers()
        => ExecutionTest("constructor-this-tdz-during-initializers");

    [Fact(DisplayName = "gen-method-length-dflt", Skip = "Tracked by issue #1055: class method function-object metadata semantics are incomplete.")]
    public Task gen_method_length_dflt()
        => ExecutionTest("gen-method-length-dflt");
}
