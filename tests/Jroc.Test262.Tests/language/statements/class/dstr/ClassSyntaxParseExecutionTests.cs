using Jroc.Test262.Tests.language.statements;

namespace Jroc.Test262.Tests.language.statements.class_.dstr;

public class ClassSyntaxParseExecutionTests : FileSystemExecutionTestsBase
{
    public ClassSyntaxParseExecutionTests() : base("language/statements/class/dstr", "language.statements.class_.dstr") { }

    [Fact(DisplayName = "async-gen-meth-ary-ptrn-rest-init-ary")]
    public Task async_gen_meth_ary_ptrn_rest_init_ary()
        => CompilationFailureTest("async-gen-meth-ary-ptrn-rest-init-ary", "Failed to parse JavaScript");

    [Fact(DisplayName = "async-gen-meth-ary-ptrn-rest-init-id")]
    public Task async_gen_meth_ary_ptrn_rest_init_id()
        => CompilationFailureTest("async-gen-meth-ary-ptrn-rest-init-id", "Failed to parse JavaScript");

    [Fact(DisplayName = "async-gen-meth-ary-ptrn-rest-init-obj")]
    public Task async_gen_meth_ary_ptrn_rest_init_obj()
        => CompilationFailureTest("async-gen-meth-ary-ptrn-rest-init-obj", "Failed to parse JavaScript");

    [Fact(DisplayName = "async-gen-meth-ary-ptrn-rest-not-final-ary")]
    public Task async_gen_meth_ary_ptrn_rest_not_final_ary()
        => CompilationFailureTest("async-gen-meth-ary-ptrn-rest-not-final-ary", "Failed to parse JavaScript");

    [Fact(DisplayName = "async-gen-meth-ary-ptrn-rest-not-final-id")]
    public Task async_gen_meth_ary_ptrn_rest_not_final_id()
        => CompilationFailureTest("async-gen-meth-ary-ptrn-rest-not-final-id", "Failed to parse JavaScript");

    [Fact(DisplayName = "async-gen-meth-ary-ptrn-rest-not-final-obj")]
    public Task async_gen_meth_ary_ptrn_rest_not_final_obj()
        => CompilationFailureTest("async-gen-meth-ary-ptrn-rest-not-final-obj", "Failed to parse JavaScript");

    [Fact(DisplayName = "async-gen-meth-dflt-ary-ptrn-rest-init-ary")]
    public Task async_gen_meth_dflt_ary_ptrn_rest_init_ary()
        => CompilationFailureTest("async-gen-meth-dflt-ary-ptrn-rest-init-ary", "Failed to parse JavaScript");

    [Fact(DisplayName = "async-gen-meth-dflt-ary-ptrn-rest-init-id")]
    public Task async_gen_meth_dflt_ary_ptrn_rest_init_id()
        => CompilationFailureTest("async-gen-meth-dflt-ary-ptrn-rest-init-id", "Failed to parse JavaScript");

    [Fact(DisplayName = "async-gen-meth-dflt-ary-ptrn-rest-init-obj")]
    public Task async_gen_meth_dflt_ary_ptrn_rest_init_obj()
        => CompilationFailureTest("async-gen-meth-dflt-ary-ptrn-rest-init-obj", "Failed to parse JavaScript");

    [Fact(DisplayName = "async-gen-meth-dflt-ary-ptrn-rest-not-final-ary")]
    public Task async_gen_meth_dflt_ary_ptrn_rest_not_final_ary()
        => CompilationFailureTest("async-gen-meth-dflt-ary-ptrn-rest-not-final-ary", "Failed to parse JavaScript");

    [Fact(DisplayName = "async-gen-meth-dflt-ary-ptrn-rest-not-final-id")]
    public Task async_gen_meth_dflt_ary_ptrn_rest_not_final_id()
        => CompilationFailureTest("async-gen-meth-dflt-ary-ptrn-rest-not-final-id", "Failed to parse JavaScript");

    [Fact(DisplayName = "async-gen-meth-dflt-ary-ptrn-rest-not-final-obj")]
    public Task async_gen_meth_dflt_ary_ptrn_rest_not_final_obj()
        => CompilationFailureTest("async-gen-meth-dflt-ary-ptrn-rest-not-final-obj", "Failed to parse JavaScript");

    [Fact(DisplayName = "async-gen-meth-static-ary-ptrn-rest-init-ary")]
    public Task async_gen_meth_static_ary_ptrn_rest_init_ary()
        => CompilationFailureTest("async-gen-meth-static-ary-ptrn-rest-init-ary", "Failed to parse JavaScript");

    [Fact(DisplayName = "async-gen-meth-static-ary-ptrn-rest-init-id")]
    public Task async_gen_meth_static_ary_ptrn_rest_init_id()
        => CompilationFailureTest("async-gen-meth-static-ary-ptrn-rest-init-id", "Failed to parse JavaScript");

    [Fact(DisplayName = "async-gen-meth-static-ary-ptrn-rest-init-obj")]
    public Task async_gen_meth_static_ary_ptrn_rest_init_obj()
        => CompilationFailureTest("async-gen-meth-static-ary-ptrn-rest-init-obj", "Failed to parse JavaScript");

    [Fact(DisplayName = "async-gen-meth-static-ary-ptrn-rest-not-final-ary")]
    public Task async_gen_meth_static_ary_ptrn_rest_not_final_ary()
        => CompilationFailureTest("async-gen-meth-static-ary-ptrn-rest-not-final-ary", "Failed to parse JavaScript");

    [Fact(DisplayName = "async-gen-meth-static-ary-ptrn-rest-not-final-id")]
    public Task async_gen_meth_static_ary_ptrn_rest_not_final_id()
        => CompilationFailureTest("async-gen-meth-static-ary-ptrn-rest-not-final-id", "Failed to parse JavaScript");

    [Fact(DisplayName = "async-gen-meth-static-ary-ptrn-rest-not-final-obj")]
    public Task async_gen_meth_static_ary_ptrn_rest_not_final_obj()
        => CompilationFailureTest("async-gen-meth-static-ary-ptrn-rest-not-final-obj", "Failed to parse JavaScript");

    [Fact(DisplayName = "async-gen-meth-static-dflt-ary-ptrn-rest-init-ary")]
    public Task async_gen_meth_static_dflt_ary_ptrn_rest_init_ary()
        => CompilationFailureTest("async-gen-meth-static-dflt-ary-ptrn-rest-init-ary", "Failed to parse JavaScript");

    [Fact(DisplayName = "async-gen-meth-static-dflt-ary-ptrn-rest-init-id")]
    public Task async_gen_meth_static_dflt_ary_ptrn_rest_init_id()
        => CompilationFailureTest("async-gen-meth-static-dflt-ary-ptrn-rest-init-id", "Failed to parse JavaScript");

    [Fact(DisplayName = "async-gen-meth-static-dflt-ary-ptrn-rest-init-obj")]
    public Task async_gen_meth_static_dflt_ary_ptrn_rest_init_obj()
        => CompilationFailureTest("async-gen-meth-static-dflt-ary-ptrn-rest-init-obj", "Failed to parse JavaScript");

    [Fact(DisplayName = "async-gen-meth-static-dflt-ary-ptrn-rest-not-final-ary")]
    public Task async_gen_meth_static_dflt_ary_ptrn_rest_not_final_ary()
        => CompilationFailureTest("async-gen-meth-static-dflt-ary-ptrn-rest-not-final-ary", "Failed to parse JavaScript");

    [Fact(DisplayName = "async-gen-meth-static-dflt-ary-ptrn-rest-not-final-id")]
    public Task async_gen_meth_static_dflt_ary_ptrn_rest_not_final_id()
        => CompilationFailureTest("async-gen-meth-static-dflt-ary-ptrn-rest-not-final-id", "Failed to parse JavaScript");

    [Fact(DisplayName = "async-gen-meth-static-dflt-ary-ptrn-rest-not-final-obj")]
    public Task async_gen_meth_static_dflt_ary_ptrn_rest_not_final_obj()
        => CompilationFailureTest("async-gen-meth-static-dflt-ary-ptrn-rest-not-final-obj", "Failed to parse JavaScript");

    [Fact(DisplayName = "async-private-gen-meth-ary-ptrn-rest-init-ary")]
    public Task async_private_gen_meth_ary_ptrn_rest_init_ary()
        => CompilationFailureTest("async-private-gen-meth-ary-ptrn-rest-init-ary", "Failed to parse JavaScript");

    [Fact(DisplayName = "async-private-gen-meth-ary-ptrn-rest-init-id")]
    public Task async_private_gen_meth_ary_ptrn_rest_init_id()
        => CompilationFailureTest("async-private-gen-meth-ary-ptrn-rest-init-id", "Failed to parse JavaScript");

    [Fact(DisplayName = "async-private-gen-meth-ary-ptrn-rest-init-obj")]
    public Task async_private_gen_meth_ary_ptrn_rest_init_obj()
        => CompilationFailureTest("async-private-gen-meth-ary-ptrn-rest-init-obj", "Failed to parse JavaScript");

    [Fact(DisplayName = "async-private-gen-meth-ary-ptrn-rest-not-final-ary")]
    public Task async_private_gen_meth_ary_ptrn_rest_not_final_ary()
        => CompilationFailureTest("async-private-gen-meth-ary-ptrn-rest-not-final-ary", "Failed to parse JavaScript");

    [Fact(DisplayName = "async-private-gen-meth-ary-ptrn-rest-not-final-id")]
    public Task async_private_gen_meth_ary_ptrn_rest_not_final_id()
        => CompilationFailureTest("async-private-gen-meth-ary-ptrn-rest-not-final-id", "Failed to parse JavaScript");

    [Fact(DisplayName = "async-private-gen-meth-ary-ptrn-rest-not-final-obj")]
    public Task async_private_gen_meth_ary_ptrn_rest_not_final_obj()
        => CompilationFailureTest("async-private-gen-meth-ary-ptrn-rest-not-final-obj", "Failed to parse JavaScript");

    [Fact(DisplayName = "async-private-gen-meth-dflt-ary-ptrn-rest-init-ary")]
    public Task async_private_gen_meth_dflt_ary_ptrn_rest_init_ary()
        => CompilationFailureTest("async-private-gen-meth-dflt-ary-ptrn-rest-init-ary", "Failed to parse JavaScript");

    [Fact(DisplayName = "async-private-gen-meth-dflt-ary-ptrn-rest-init-id")]
    public Task async_private_gen_meth_dflt_ary_ptrn_rest_init_id()
        => CompilationFailureTest("async-private-gen-meth-dflt-ary-ptrn-rest-init-id", "Failed to parse JavaScript");

    [Fact(DisplayName = "async-private-gen-meth-dflt-ary-ptrn-rest-init-obj")]
    public Task async_private_gen_meth_dflt_ary_ptrn_rest_init_obj()
        => CompilationFailureTest("async-private-gen-meth-dflt-ary-ptrn-rest-init-obj", "Failed to parse JavaScript");

    [Fact(DisplayName = "async-private-gen-meth-dflt-ary-ptrn-rest-not-final-ary")]
    public Task async_private_gen_meth_dflt_ary_ptrn_rest_not_final_ary()
        => CompilationFailureTest("async-private-gen-meth-dflt-ary-ptrn-rest-not-final-ary", "Failed to parse JavaScript");

    [Fact(DisplayName = "async-private-gen-meth-dflt-ary-ptrn-rest-not-final-id")]
    public Task async_private_gen_meth_dflt_ary_ptrn_rest_not_final_id()
        => CompilationFailureTest("async-private-gen-meth-dflt-ary-ptrn-rest-not-final-id", "Failed to parse JavaScript");

    [Fact(DisplayName = "async-private-gen-meth-dflt-ary-ptrn-rest-not-final-obj")]
    public Task async_private_gen_meth_dflt_ary_ptrn_rest_not_final_obj()
        => CompilationFailureTest("async-private-gen-meth-dflt-ary-ptrn-rest-not-final-obj", "Failed to parse JavaScript");

    [Fact(DisplayName = "async-private-gen-meth-static-ary-ptrn-rest-init-ary")]
    public Task async_private_gen_meth_static_ary_ptrn_rest_init_ary()
        => CompilationFailureTest("async-private-gen-meth-static-ary-ptrn-rest-init-ary", "Failed to parse JavaScript");

    [Fact(DisplayName = "async-private-gen-meth-static-ary-ptrn-rest-init-id")]
    public Task async_private_gen_meth_static_ary_ptrn_rest_init_id()
        => CompilationFailureTest("async-private-gen-meth-static-ary-ptrn-rest-init-id", "Failed to parse JavaScript");

    [Fact(DisplayName = "async-private-gen-meth-static-ary-ptrn-rest-init-obj")]
    public Task async_private_gen_meth_static_ary_ptrn_rest_init_obj()
        => CompilationFailureTest("async-private-gen-meth-static-ary-ptrn-rest-init-obj", "Failed to parse JavaScript");

    [Fact(DisplayName = "async-private-gen-meth-static-ary-ptrn-rest-not-final-ary")]
    public Task async_private_gen_meth_static_ary_ptrn_rest_not_final_ary()
        => CompilationFailureTest("async-private-gen-meth-static-ary-ptrn-rest-not-final-ary", "Failed to parse JavaScript");

    [Fact(DisplayName = "async-private-gen-meth-static-ary-ptrn-rest-not-final-id")]
    public Task async_private_gen_meth_static_ary_ptrn_rest_not_final_id()
        => CompilationFailureTest("async-private-gen-meth-static-ary-ptrn-rest-not-final-id", "Failed to parse JavaScript");

    [Fact(DisplayName = "async-private-gen-meth-static-ary-ptrn-rest-not-final-obj")]
    public Task async_private_gen_meth_static_ary_ptrn_rest_not_final_obj()
        => CompilationFailureTest("async-private-gen-meth-static-ary-ptrn-rest-not-final-obj", "Failed to parse JavaScript");

    [Fact(DisplayName = "async-private-gen-meth-static-dflt-ary-ptrn-rest-init-ary")]
    public Task async_private_gen_meth_static_dflt_ary_ptrn_rest_init_ary()
        => CompilationFailureTest("async-private-gen-meth-static-dflt-ary-ptrn-rest-init-ary", "Failed to parse JavaScript");

    [Fact(DisplayName = "async-private-gen-meth-static-dflt-ary-ptrn-rest-init-id")]
    public Task async_private_gen_meth_static_dflt_ary_ptrn_rest_init_id()
        => CompilationFailureTest("async-private-gen-meth-static-dflt-ary-ptrn-rest-init-id", "Failed to parse JavaScript");

    [Fact(DisplayName = "async-private-gen-meth-static-dflt-ary-ptrn-rest-init-obj")]
    public Task async_private_gen_meth_static_dflt_ary_ptrn_rest_init_obj()
        => CompilationFailureTest("async-private-gen-meth-static-dflt-ary-ptrn-rest-init-obj", "Failed to parse JavaScript");

    [Fact(DisplayName = "async-private-gen-meth-static-dflt-ary-ptrn-rest-not-final-ary")]
    public Task async_private_gen_meth_static_dflt_ary_ptrn_rest_not_final_ary()
        => CompilationFailureTest("async-private-gen-meth-static-dflt-ary-ptrn-rest-not-final-ary", "Failed to parse JavaScript");

    [Fact(DisplayName = "async-private-gen-meth-static-dflt-ary-ptrn-rest-not-final-id")]
    public Task async_private_gen_meth_static_dflt_ary_ptrn_rest_not_final_id()
        => CompilationFailureTest("async-private-gen-meth-static-dflt-ary-ptrn-rest-not-final-id", "Failed to parse JavaScript");

    [Fact(DisplayName = "async-private-gen-meth-static-dflt-ary-ptrn-rest-not-final-obj")]
    public Task async_private_gen_meth_static_dflt_ary_ptrn_rest_not_final_obj()
        => CompilationFailureTest("async-private-gen-meth-static-dflt-ary-ptrn-rest-not-final-obj", "Failed to parse JavaScript");

    [Fact(DisplayName = "gen-meth-ary-ptrn-rest-init-ary")]
    public Task gen_meth_ary_ptrn_rest_init_ary()
        => CompilationFailureTest("gen-meth-ary-ptrn-rest-init-ary", "Failed to parse JavaScript");

    [Fact(DisplayName = "gen-meth-ary-ptrn-rest-init-id")]
    public Task gen_meth_ary_ptrn_rest_init_id()
        => CompilationFailureTest("gen-meth-ary-ptrn-rest-init-id", "Failed to parse JavaScript");

    [Fact(DisplayName = "gen-meth-ary-ptrn-rest-init-obj")]
    public Task gen_meth_ary_ptrn_rest_init_obj()
        => CompilationFailureTest("gen-meth-ary-ptrn-rest-init-obj", "Failed to parse JavaScript");

    [Fact(DisplayName = "gen-meth-ary-ptrn-rest-not-final-ary")]
    public Task gen_meth_ary_ptrn_rest_not_final_ary()
        => CompilationFailureTest("gen-meth-ary-ptrn-rest-not-final-ary", "Failed to parse JavaScript");

    [Fact(DisplayName = "gen-meth-ary-ptrn-rest-not-final-id")]
    public Task gen_meth_ary_ptrn_rest_not_final_id()
        => CompilationFailureTest("gen-meth-ary-ptrn-rest-not-final-id", "Failed to parse JavaScript");

    [Fact(DisplayName = "gen-meth-ary-ptrn-rest-not-final-obj")]
    public Task gen_meth_ary_ptrn_rest_not_final_obj()
        => CompilationFailureTest("gen-meth-ary-ptrn-rest-not-final-obj", "Failed to parse JavaScript");

    [Fact(DisplayName = "gen-meth-dflt-ary-ptrn-rest-init-ary")]
    public Task gen_meth_dflt_ary_ptrn_rest_init_ary()
        => CompilationFailureTest("gen-meth-dflt-ary-ptrn-rest-init-ary", "Failed to parse JavaScript");

    [Fact(DisplayName = "gen-meth-dflt-ary-ptrn-rest-init-id")]
    public Task gen_meth_dflt_ary_ptrn_rest_init_id()
        => CompilationFailureTest("gen-meth-dflt-ary-ptrn-rest-init-id", "Failed to parse JavaScript");

    [Fact(DisplayName = "gen-meth-dflt-ary-ptrn-rest-init-obj")]
    public Task gen_meth_dflt_ary_ptrn_rest_init_obj()
        => CompilationFailureTest("gen-meth-dflt-ary-ptrn-rest-init-obj", "Failed to parse JavaScript");

    [Fact(DisplayName = "gen-meth-dflt-ary-ptrn-rest-not-final-ary")]
    public Task gen_meth_dflt_ary_ptrn_rest_not_final_ary()
        => CompilationFailureTest("gen-meth-dflt-ary-ptrn-rest-not-final-ary", "Failed to parse JavaScript");

    [Fact(DisplayName = "gen-meth-dflt-ary-ptrn-rest-not-final-id")]
    public Task gen_meth_dflt_ary_ptrn_rest_not_final_id()
        => CompilationFailureTest("gen-meth-dflt-ary-ptrn-rest-not-final-id", "Failed to parse JavaScript");

    [Fact(DisplayName = "gen-meth-dflt-ary-ptrn-rest-not-final-obj")]
    public Task gen_meth_dflt_ary_ptrn_rest_not_final_obj()
        => CompilationFailureTest("gen-meth-dflt-ary-ptrn-rest-not-final-obj", "Failed to parse JavaScript");

    [Fact(DisplayName = "gen-meth-static-ary-ptrn-rest-init-ary")]
    public Task gen_meth_static_ary_ptrn_rest_init_ary()
        => CompilationFailureTest("gen-meth-static-ary-ptrn-rest-init-ary", "Failed to parse JavaScript");

    [Fact(DisplayName = "gen-meth-static-ary-ptrn-rest-init-id")]
    public Task gen_meth_static_ary_ptrn_rest_init_id()
        => CompilationFailureTest("gen-meth-static-ary-ptrn-rest-init-id", "Failed to parse JavaScript");

    [Fact(DisplayName = "gen-meth-static-ary-ptrn-rest-init-obj")]
    public Task gen_meth_static_ary_ptrn_rest_init_obj()
        => CompilationFailureTest("gen-meth-static-ary-ptrn-rest-init-obj", "Failed to parse JavaScript");

    [Fact(DisplayName = "gen-meth-static-ary-ptrn-rest-not-final-ary")]
    public Task gen_meth_static_ary_ptrn_rest_not_final_ary()
        => CompilationFailureTest("gen-meth-static-ary-ptrn-rest-not-final-ary", "Failed to parse JavaScript");

    [Fact(DisplayName = "gen-meth-static-ary-ptrn-rest-not-final-id")]
    public Task gen_meth_static_ary_ptrn_rest_not_final_id()
        => CompilationFailureTest("gen-meth-static-ary-ptrn-rest-not-final-id", "Failed to parse JavaScript");

    [Fact(DisplayName = "gen-meth-static-ary-ptrn-rest-not-final-obj")]
    public Task gen_meth_static_ary_ptrn_rest_not_final_obj()
        => CompilationFailureTest("gen-meth-static-ary-ptrn-rest-not-final-obj", "Failed to parse JavaScript");

    [Fact(DisplayName = "gen-meth-static-dflt-ary-ptrn-rest-init-ary")]
    public Task gen_meth_static_dflt_ary_ptrn_rest_init_ary()
        => CompilationFailureTest("gen-meth-static-dflt-ary-ptrn-rest-init-ary", "Failed to parse JavaScript");

    [Fact(DisplayName = "gen-meth-static-dflt-ary-ptrn-rest-init-id")]
    public Task gen_meth_static_dflt_ary_ptrn_rest_init_id()
        => CompilationFailureTest("gen-meth-static-dflt-ary-ptrn-rest-init-id", "Failed to parse JavaScript");

    [Fact(DisplayName = "gen-meth-static-dflt-ary-ptrn-rest-init-obj")]
    public Task gen_meth_static_dflt_ary_ptrn_rest_init_obj()
        => CompilationFailureTest("gen-meth-static-dflt-ary-ptrn-rest-init-obj", "Failed to parse JavaScript");

    [Fact(DisplayName = "gen-meth-static-dflt-ary-ptrn-rest-not-final-ary")]
    public Task gen_meth_static_dflt_ary_ptrn_rest_not_final_ary()
        => CompilationFailureTest("gen-meth-static-dflt-ary-ptrn-rest-not-final-ary", "Failed to parse JavaScript");

    [Fact(DisplayName = "gen-meth-static-dflt-ary-ptrn-rest-not-final-id")]
    public Task gen_meth_static_dflt_ary_ptrn_rest_not_final_id()
        => CompilationFailureTest("gen-meth-static-dflt-ary-ptrn-rest-not-final-id", "Failed to parse JavaScript");

    [Fact(DisplayName = "gen-meth-static-dflt-ary-ptrn-rest-not-final-obj")]
    public Task gen_meth_static_dflt_ary_ptrn_rest_not_final_obj()
        => CompilationFailureTest("gen-meth-static-dflt-ary-ptrn-rest-not-final-obj", "Failed to parse JavaScript");

    [Fact(DisplayName = "meth-ary-ptrn-rest-init-ary")]
    public Task meth_ary_ptrn_rest_init_ary()
        => CompilationFailureTest("meth-ary-ptrn-rest-init-ary", "Failed to parse JavaScript");

    [Fact(DisplayName = "meth-ary-ptrn-rest-init-id")]
    public Task meth_ary_ptrn_rest_init_id()
        => CompilationFailureTest("meth-ary-ptrn-rest-init-id", "Failed to parse JavaScript");

    [Fact(DisplayName = "meth-ary-ptrn-rest-init-obj")]
    public Task meth_ary_ptrn_rest_init_obj()
        => CompilationFailureTest("meth-ary-ptrn-rest-init-obj", "Failed to parse JavaScript");

    [Fact(DisplayName = "meth-ary-ptrn-rest-not-final-ary")]
    public Task meth_ary_ptrn_rest_not_final_ary()
        => CompilationFailureTest("meth-ary-ptrn-rest-not-final-ary", "Failed to parse JavaScript");

    [Fact(DisplayName = "meth-ary-ptrn-rest-not-final-id")]
    public Task meth_ary_ptrn_rest_not_final_id()
        => CompilationFailureTest("meth-ary-ptrn-rest-not-final-id", "Failed to parse JavaScript");

    [Fact(DisplayName = "meth-ary-ptrn-rest-not-final-obj")]
    public Task meth_ary_ptrn_rest_not_final_obj()
        => CompilationFailureTest("meth-ary-ptrn-rest-not-final-obj", "Failed to parse JavaScript");

    [Fact(DisplayName = "meth-dflt-ary-ptrn-rest-init-ary")]
    public Task meth_dflt_ary_ptrn_rest_init_ary()
        => CompilationFailureTest("meth-dflt-ary-ptrn-rest-init-ary", "Failed to parse JavaScript");

    [Fact(DisplayName = "meth-dflt-ary-ptrn-rest-init-id")]
    public Task meth_dflt_ary_ptrn_rest_init_id()
        => CompilationFailureTest("meth-dflt-ary-ptrn-rest-init-id", "Failed to parse JavaScript");

    [Fact(DisplayName = "meth-dflt-ary-ptrn-rest-init-obj")]
    public Task meth_dflt_ary_ptrn_rest_init_obj()
        => CompilationFailureTest("meth-dflt-ary-ptrn-rest-init-obj", "Failed to parse JavaScript");

    [Fact(DisplayName = "meth-dflt-ary-ptrn-rest-not-final-ary")]
    public Task meth_dflt_ary_ptrn_rest_not_final_ary()
        => CompilationFailureTest("meth-dflt-ary-ptrn-rest-not-final-ary", "Failed to parse JavaScript");

    [Fact(DisplayName = "meth-dflt-ary-ptrn-rest-not-final-id")]
    public Task meth_dflt_ary_ptrn_rest_not_final_id()
        => CompilationFailureTest("meth-dflt-ary-ptrn-rest-not-final-id", "Failed to parse JavaScript");

    [Fact(DisplayName = "meth-dflt-ary-ptrn-rest-not-final-obj")]
    public Task meth_dflt_ary_ptrn_rest_not_final_obj()
        => CompilationFailureTest("meth-dflt-ary-ptrn-rest-not-final-obj", "Failed to parse JavaScript");

    [Fact(DisplayName = "meth-static-ary-ptrn-rest-init-ary")]
    public Task meth_static_ary_ptrn_rest_init_ary()
        => CompilationFailureTest("meth-static-ary-ptrn-rest-init-ary", "Failed to parse JavaScript");

    [Fact(DisplayName = "meth-static-ary-ptrn-rest-init-id")]
    public Task meth_static_ary_ptrn_rest_init_id()
        => CompilationFailureTest("meth-static-ary-ptrn-rest-init-id", "Failed to parse JavaScript");

    [Fact(DisplayName = "meth-static-ary-ptrn-rest-init-obj")]
    public Task meth_static_ary_ptrn_rest_init_obj()
        => CompilationFailureTest("meth-static-ary-ptrn-rest-init-obj", "Failed to parse JavaScript");

    [Fact(DisplayName = "meth-static-ary-ptrn-rest-not-final-ary")]
    public Task meth_static_ary_ptrn_rest_not_final_ary()
        => CompilationFailureTest("meth-static-ary-ptrn-rest-not-final-ary", "Failed to parse JavaScript");

    [Fact(DisplayName = "meth-static-ary-ptrn-rest-not-final-id")]
    public Task meth_static_ary_ptrn_rest_not_final_id()
        => CompilationFailureTest("meth-static-ary-ptrn-rest-not-final-id", "Failed to parse JavaScript");

    [Fact(DisplayName = "meth-static-ary-ptrn-rest-not-final-obj")]
    public Task meth_static_ary_ptrn_rest_not_final_obj()
        => CompilationFailureTest("meth-static-ary-ptrn-rest-not-final-obj", "Failed to parse JavaScript");

    [Fact(DisplayName = "meth-static-dflt-ary-ptrn-rest-init-ary")]
    public Task meth_static_dflt_ary_ptrn_rest_init_ary()
        => CompilationFailureTest("meth-static-dflt-ary-ptrn-rest-init-ary", "Failed to parse JavaScript");

    [Fact(DisplayName = "meth-static-dflt-ary-ptrn-rest-init-id")]
    public Task meth_static_dflt_ary_ptrn_rest_init_id()
        => CompilationFailureTest("meth-static-dflt-ary-ptrn-rest-init-id", "Failed to parse JavaScript");

    [Fact(DisplayName = "meth-static-dflt-ary-ptrn-rest-init-obj")]
    public Task meth_static_dflt_ary_ptrn_rest_init_obj()
        => CompilationFailureTest("meth-static-dflt-ary-ptrn-rest-init-obj", "Failed to parse JavaScript");

    [Fact(DisplayName = "meth-static-dflt-ary-ptrn-rest-not-final-ary")]
    public Task meth_static_dflt_ary_ptrn_rest_not_final_ary()
        => CompilationFailureTest("meth-static-dflt-ary-ptrn-rest-not-final-ary", "Failed to parse JavaScript");

    [Fact(DisplayName = "meth-static-dflt-ary-ptrn-rest-not-final-id")]
    public Task meth_static_dflt_ary_ptrn_rest_not_final_id()
        => CompilationFailureTest("meth-static-dflt-ary-ptrn-rest-not-final-id", "Failed to parse JavaScript");

    [Fact(DisplayName = "meth-static-dflt-ary-ptrn-rest-not-final-obj")]
    public Task meth_static_dflt_ary_ptrn_rest_not_final_obj()
        => CompilationFailureTest("meth-static-dflt-ary-ptrn-rest-not-final-obj", "Failed to parse JavaScript");

    [Fact(DisplayName = "private-gen-meth-ary-ptrn-rest-init-ary")]
    public Task private_gen_meth_ary_ptrn_rest_init_ary()
        => CompilationFailureTest("private-gen-meth-ary-ptrn-rest-init-ary", "Failed to parse JavaScript");

    [Fact(DisplayName = "private-gen-meth-ary-ptrn-rest-init-id")]
    public Task private_gen_meth_ary_ptrn_rest_init_id()
        => CompilationFailureTest("private-gen-meth-ary-ptrn-rest-init-id", "Failed to parse JavaScript");

    [Fact(DisplayName = "private-gen-meth-ary-ptrn-rest-init-obj")]
    public Task private_gen_meth_ary_ptrn_rest_init_obj()
        => CompilationFailureTest("private-gen-meth-ary-ptrn-rest-init-obj", "Failed to parse JavaScript");

    [Fact(DisplayName = "private-gen-meth-ary-ptrn-rest-not-final-ary")]
    public Task private_gen_meth_ary_ptrn_rest_not_final_ary()
        => CompilationFailureTest("private-gen-meth-ary-ptrn-rest-not-final-ary", "Failed to parse JavaScript");

    [Fact(DisplayName = "private-gen-meth-ary-ptrn-rest-not-final-id")]
    public Task private_gen_meth_ary_ptrn_rest_not_final_id()
        => CompilationFailureTest("private-gen-meth-ary-ptrn-rest-not-final-id", "Failed to parse JavaScript");

    [Fact(DisplayName = "private-gen-meth-ary-ptrn-rest-not-final-obj")]
    public Task private_gen_meth_ary_ptrn_rest_not_final_obj()
        => CompilationFailureTest("private-gen-meth-ary-ptrn-rest-not-final-obj", "Failed to parse JavaScript");

    [Fact(DisplayName = "private-gen-meth-dflt-ary-ptrn-rest-init-ary")]
    public Task private_gen_meth_dflt_ary_ptrn_rest_init_ary()
        => CompilationFailureTest("private-gen-meth-dflt-ary-ptrn-rest-init-ary", "Failed to parse JavaScript");

    [Fact(DisplayName = "private-gen-meth-dflt-ary-ptrn-rest-init-id")]
    public Task private_gen_meth_dflt_ary_ptrn_rest_init_id()
        => CompilationFailureTest("private-gen-meth-dflt-ary-ptrn-rest-init-id", "Failed to parse JavaScript");

    [Fact(DisplayName = "private-gen-meth-dflt-ary-ptrn-rest-init-obj")]
    public Task private_gen_meth_dflt_ary_ptrn_rest_init_obj()
        => CompilationFailureTest("private-gen-meth-dflt-ary-ptrn-rest-init-obj", "Failed to parse JavaScript");

    [Fact(DisplayName = "private-gen-meth-dflt-ary-ptrn-rest-not-final-ary")]
    public Task private_gen_meth_dflt_ary_ptrn_rest_not_final_ary()
        => CompilationFailureTest("private-gen-meth-dflt-ary-ptrn-rest-not-final-ary", "Failed to parse JavaScript");

    [Fact(DisplayName = "private-gen-meth-dflt-ary-ptrn-rest-not-final-id")]
    public Task private_gen_meth_dflt_ary_ptrn_rest_not_final_id()
        => CompilationFailureTest("private-gen-meth-dflt-ary-ptrn-rest-not-final-id", "Failed to parse JavaScript");

    [Fact(DisplayName = "private-gen-meth-dflt-ary-ptrn-rest-not-final-obj")]
    public Task private_gen_meth_dflt_ary_ptrn_rest_not_final_obj()
        => CompilationFailureTest("private-gen-meth-dflt-ary-ptrn-rest-not-final-obj", "Failed to parse JavaScript");

    [Fact(DisplayName = "private-gen-meth-static-ary-ptrn-rest-init-ary")]
    public Task private_gen_meth_static_ary_ptrn_rest_init_ary()
        => CompilationFailureTest("private-gen-meth-static-ary-ptrn-rest-init-ary", "Failed to parse JavaScript");

    [Fact(DisplayName = "private-gen-meth-static-ary-ptrn-rest-init-id")]
    public Task private_gen_meth_static_ary_ptrn_rest_init_id()
        => CompilationFailureTest("private-gen-meth-static-ary-ptrn-rest-init-id", "Failed to parse JavaScript");

    [Fact(DisplayName = "private-gen-meth-static-ary-ptrn-rest-init-obj")]
    public Task private_gen_meth_static_ary_ptrn_rest_init_obj()
        => CompilationFailureTest("private-gen-meth-static-ary-ptrn-rest-init-obj", "Failed to parse JavaScript");

    [Fact(DisplayName = "private-gen-meth-static-ary-ptrn-rest-not-final-ary")]
    public Task private_gen_meth_static_ary_ptrn_rest_not_final_ary()
        => CompilationFailureTest("private-gen-meth-static-ary-ptrn-rest-not-final-ary", "Failed to parse JavaScript");

    [Fact(DisplayName = "private-gen-meth-static-ary-ptrn-rest-not-final-id")]
    public Task private_gen_meth_static_ary_ptrn_rest_not_final_id()
        => CompilationFailureTest("private-gen-meth-static-ary-ptrn-rest-not-final-id", "Failed to parse JavaScript");

    [Fact(DisplayName = "private-gen-meth-static-ary-ptrn-rest-not-final-obj")]
    public Task private_gen_meth_static_ary_ptrn_rest_not_final_obj()
        => CompilationFailureTest("private-gen-meth-static-ary-ptrn-rest-not-final-obj", "Failed to parse JavaScript");

    [Fact(DisplayName = "private-gen-meth-static-dflt-ary-ptrn-rest-init-ary")]
    public Task private_gen_meth_static_dflt_ary_ptrn_rest_init_ary()
        => CompilationFailureTest("private-gen-meth-static-dflt-ary-ptrn-rest-init-ary", "Failed to parse JavaScript");

    [Fact(DisplayName = "private-gen-meth-static-dflt-ary-ptrn-rest-init-id")]
    public Task private_gen_meth_static_dflt_ary_ptrn_rest_init_id()
        => CompilationFailureTest("private-gen-meth-static-dflt-ary-ptrn-rest-init-id", "Failed to parse JavaScript");

    [Fact(DisplayName = "private-gen-meth-static-dflt-ary-ptrn-rest-init-obj")]
    public Task private_gen_meth_static_dflt_ary_ptrn_rest_init_obj()
        => CompilationFailureTest("private-gen-meth-static-dflt-ary-ptrn-rest-init-obj", "Failed to parse JavaScript");

    [Fact(DisplayName = "private-gen-meth-static-dflt-ary-ptrn-rest-not-final-ary")]
    public Task private_gen_meth_static_dflt_ary_ptrn_rest_not_final_ary()
        => CompilationFailureTest("private-gen-meth-static-dflt-ary-ptrn-rest-not-final-ary", "Failed to parse JavaScript");

    [Fact(DisplayName = "private-gen-meth-static-dflt-ary-ptrn-rest-not-final-id")]
    public Task private_gen_meth_static_dflt_ary_ptrn_rest_not_final_id()
        => CompilationFailureTest("private-gen-meth-static-dflt-ary-ptrn-rest-not-final-id", "Failed to parse JavaScript");

    [Fact(DisplayName = "private-gen-meth-static-dflt-ary-ptrn-rest-not-final-obj")]
    public Task private_gen_meth_static_dflt_ary_ptrn_rest_not_final_obj()
        => CompilationFailureTest("private-gen-meth-static-dflt-ary-ptrn-rest-not-final-obj", "Failed to parse JavaScript");

    [Fact(DisplayName = "private-meth-ary-ptrn-rest-init-ary")]
    public Task private_meth_ary_ptrn_rest_init_ary()
        => CompilationFailureTest("private-meth-ary-ptrn-rest-init-ary", "Failed to parse JavaScript");

    [Fact(DisplayName = "private-meth-ary-ptrn-rest-init-id")]
    public Task private_meth_ary_ptrn_rest_init_id()
        => CompilationFailureTest("private-meth-ary-ptrn-rest-init-id", "Failed to parse JavaScript");

    [Fact(DisplayName = "private-meth-ary-ptrn-rest-init-obj")]
    public Task private_meth_ary_ptrn_rest_init_obj()
        => CompilationFailureTest("private-meth-ary-ptrn-rest-init-obj", "Failed to parse JavaScript");

    [Fact(DisplayName = "private-meth-ary-ptrn-rest-not-final-ary")]
    public Task private_meth_ary_ptrn_rest_not_final_ary()
        => CompilationFailureTest("private-meth-ary-ptrn-rest-not-final-ary", "Failed to parse JavaScript");

    [Fact(DisplayName = "private-meth-ary-ptrn-rest-not-final-id")]
    public Task private_meth_ary_ptrn_rest_not_final_id()
        => CompilationFailureTest("private-meth-ary-ptrn-rest-not-final-id", "Failed to parse JavaScript");

    [Fact(DisplayName = "private-meth-ary-ptrn-rest-not-final-obj")]
    public Task private_meth_ary_ptrn_rest_not_final_obj()
        => CompilationFailureTest("private-meth-ary-ptrn-rest-not-final-obj", "Failed to parse JavaScript");

    [Fact(DisplayName = "private-meth-dflt-ary-ptrn-rest-init-ary")]
    public Task private_meth_dflt_ary_ptrn_rest_init_ary()
        => CompilationFailureTest("private-meth-dflt-ary-ptrn-rest-init-ary", "Failed to parse JavaScript");

    [Fact(DisplayName = "private-meth-dflt-ary-ptrn-rest-init-id")]
    public Task private_meth_dflt_ary_ptrn_rest_init_id()
        => CompilationFailureTest("private-meth-dflt-ary-ptrn-rest-init-id", "Failed to parse JavaScript");

    [Fact(DisplayName = "private-meth-dflt-ary-ptrn-rest-init-obj")]
    public Task private_meth_dflt_ary_ptrn_rest_init_obj()
        => CompilationFailureTest("private-meth-dflt-ary-ptrn-rest-init-obj", "Failed to parse JavaScript");

    [Fact(DisplayName = "private-meth-dflt-ary-ptrn-rest-not-final-ary")]
    public Task private_meth_dflt_ary_ptrn_rest_not_final_ary()
        => CompilationFailureTest("private-meth-dflt-ary-ptrn-rest-not-final-ary", "Failed to parse JavaScript");

    [Fact(DisplayName = "private-meth-dflt-ary-ptrn-rest-not-final-id")]
    public Task private_meth_dflt_ary_ptrn_rest_not_final_id()
        => CompilationFailureTest("private-meth-dflt-ary-ptrn-rest-not-final-id", "Failed to parse JavaScript");

    [Fact(DisplayName = "private-meth-dflt-ary-ptrn-rest-not-final-obj")]
    public Task private_meth_dflt_ary_ptrn_rest_not_final_obj()
        => CompilationFailureTest("private-meth-dflt-ary-ptrn-rest-not-final-obj", "Failed to parse JavaScript");

    [Fact(DisplayName = "private-meth-static-ary-ptrn-rest-init-ary")]
    public Task private_meth_static_ary_ptrn_rest_init_ary()
        => CompilationFailureTest("private-meth-static-ary-ptrn-rest-init-ary", "Failed to parse JavaScript");

    [Fact(DisplayName = "private-meth-static-ary-ptrn-rest-init-id")]
    public Task private_meth_static_ary_ptrn_rest_init_id()
        => CompilationFailureTest("private-meth-static-ary-ptrn-rest-init-id", "Failed to parse JavaScript");

    [Fact(DisplayName = "private-meth-static-ary-ptrn-rest-init-obj")]
    public Task private_meth_static_ary_ptrn_rest_init_obj()
        => CompilationFailureTest("private-meth-static-ary-ptrn-rest-init-obj", "Failed to parse JavaScript");

    [Fact(DisplayName = "private-meth-static-ary-ptrn-rest-not-final-ary")]
    public Task private_meth_static_ary_ptrn_rest_not_final_ary()
        => CompilationFailureTest("private-meth-static-ary-ptrn-rest-not-final-ary", "Failed to parse JavaScript");

    [Fact(DisplayName = "private-meth-static-ary-ptrn-rest-not-final-id")]
    public Task private_meth_static_ary_ptrn_rest_not_final_id()
        => CompilationFailureTest("private-meth-static-ary-ptrn-rest-not-final-id", "Failed to parse JavaScript");

    [Fact(DisplayName = "private-meth-static-ary-ptrn-rest-not-final-obj")]
    public Task private_meth_static_ary_ptrn_rest_not_final_obj()
        => CompilationFailureTest("private-meth-static-ary-ptrn-rest-not-final-obj", "Failed to parse JavaScript");

    [Fact(DisplayName = "private-meth-static-dflt-ary-ptrn-rest-init-ary")]
    public Task private_meth_static_dflt_ary_ptrn_rest_init_ary()
        => CompilationFailureTest("private-meth-static-dflt-ary-ptrn-rest-init-ary", "Failed to parse JavaScript");

    [Fact(DisplayName = "private-meth-static-dflt-ary-ptrn-rest-init-id")]
    public Task private_meth_static_dflt_ary_ptrn_rest_init_id()
        => CompilationFailureTest("private-meth-static-dflt-ary-ptrn-rest-init-id", "Failed to parse JavaScript");

    [Fact(DisplayName = "private-meth-static-dflt-ary-ptrn-rest-init-obj")]
    public Task private_meth_static_dflt_ary_ptrn_rest_init_obj()
        => CompilationFailureTest("private-meth-static-dflt-ary-ptrn-rest-init-obj", "Failed to parse JavaScript");

    [Fact(DisplayName = "private-meth-static-dflt-ary-ptrn-rest-not-final-ary")]
    public Task private_meth_static_dflt_ary_ptrn_rest_not_final_ary()
        => CompilationFailureTest("private-meth-static-dflt-ary-ptrn-rest-not-final-ary", "Failed to parse JavaScript");

    [Fact(DisplayName = "private-meth-static-dflt-ary-ptrn-rest-not-final-id")]
    public Task private_meth_static_dflt_ary_ptrn_rest_not_final_id()
        => CompilationFailureTest("private-meth-static-dflt-ary-ptrn-rest-not-final-id", "Failed to parse JavaScript");

    [Fact(DisplayName = "private-meth-static-dflt-ary-ptrn-rest-not-final-obj")]
    public Task private_meth_static_dflt_ary_ptrn_rest_not_final_obj()
        => CompilationFailureTest("private-meth-static-dflt-ary-ptrn-rest-not-final-obj", "Failed to parse JavaScript");
}
