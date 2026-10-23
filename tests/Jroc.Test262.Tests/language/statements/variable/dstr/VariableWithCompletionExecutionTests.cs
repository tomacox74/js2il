namespace Jroc.Test262.Tests.language.statements.variable.dstr;

public sealed class VariableWithCompletionExecutionTests : DiskExecutionTestsBase
{
    public VariableWithCompletionExecutionTests() : base("language.statements.variable.dstr") { }

    [Fact(DisplayName = "ary-ptrn-elem-id-static-init-await-invalid.js")]
    public Task ary_ptrn_elem_id_static_init_await_invalid()
        => CompilationFailureTest("ary-ptrn-elem-id-static-init-await-invalid", "Failed to parse JavaScript");

    [Fact(DisplayName = "ary-ptrn-elem-id-static-init-await-valid.js")]
    public Task ary_ptrn_elem_id_static_init_await_valid()
        => ExecutionTest("ary-ptrn-elem-id-static-init-await-valid");

    [Fact(DisplayName = "ary-ptrn-rest-init-ary.js")]
    public Task ary_ptrn_rest_init_ary()
        => CompilationFailureTest("ary-ptrn-rest-init-ary", "Failed to parse JavaScript");

    [Fact(DisplayName = "ary-ptrn-rest-init-id.js")]
    public Task ary_ptrn_rest_init_id()
        => CompilationFailureTest("ary-ptrn-rest-init-id", "Failed to parse JavaScript");

    [Fact(DisplayName = "ary-ptrn-rest-init-obj.js")]
    public Task ary_ptrn_rest_init_obj()
        => CompilationFailureTest("ary-ptrn-rest-init-obj", "Failed to parse JavaScript");

    [Fact(DisplayName = "ary-ptrn-rest-not-final-ary.js")]
    public Task ary_ptrn_rest_not_final_ary()
        => CompilationFailureTest("ary-ptrn-rest-not-final-ary", "Failed to parse JavaScript");

    [Fact(DisplayName = "ary-ptrn-rest-not-final-id.js")]
    public Task ary_ptrn_rest_not_final_id()
        => CompilationFailureTest("ary-ptrn-rest-not-final-id", "Failed to parse JavaScript");

    [Fact(DisplayName = "ary-ptrn-rest-not-final-obj.js")]
    public Task ary_ptrn_rest_not_final_obj()
        => CompilationFailureTest("ary-ptrn-rest-not-final-obj", "Failed to parse JavaScript");

    [Fact(DisplayName = "obj-ptrn-elem-id-static-init-await-invalid.js")]
    public Task obj_ptrn_elem_id_static_init_await_invalid()
        => CompilationFailureTest("obj-ptrn-elem-id-static-init-await-invalid", "Failed to parse JavaScript");

    [Fact(DisplayName = "obj-ptrn-elem-id-static-init-await-valid.js")]
    public Task obj_ptrn_elem_id_static_init_await_valid()
        => ExecutionTest("obj-ptrn-elem-id-static-init-await-valid");
}
