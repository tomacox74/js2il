namespace Jroc.Test262.Tests.built_ins.String.prototype.matchAll;

public partial class ExecutionTests
{
    [Fact(DisplayName = "cstm-matchall-on-boolean-primitive.js")]
    public Task cstm_matchall_on_boolean_primitive() => ExecutionTestFromFile("cstm-matchall-on-boolean-primitive");
    [Fact(DisplayName = "flags-nonglobal-throws.js")]
    public Task flags_nonglobal_throws() => ExecutionTestFromFile("flags-nonglobal-throws");
    [Fact(DisplayName = "length.js")]
    public Task length() => ExecutionTestFromFile("length");
    [Fact(DisplayName = "name.js")]
    public Task name() => ExecutionTestFromFile("name");
    [Fact(DisplayName = "not-a-constructor.js")]
    public Task not_a_constructor() => ExecutionTestFromFile("not-a-constructor");
    [Fact(DisplayName = "prop-desc.js")]
    public Task prop_desc() => ExecutionTestFromFile("prop-desc");
    [Fact(DisplayName = "regexp-matchAll-invocation.js")]
    public Task regexp_matchAll_invocation() => ExecutionTestFromFile("regexp-matchAll-invocation");
    [Fact(DisplayName = "regexp-matchAll-is-undefined-or-null.js")]
    public Task regexp_matchAll_is_undefined_or_null() => ExecutionTestFromFile("regexp-matchAll-is-undefined-or-null");
    [Fact(DisplayName = "regexp-matchAll-not-callable.js")]
    public Task regexp_matchAll_not_callable() => ExecutionTestFromFile("regexp-matchAll-not-callable");
    [Fact(DisplayName = "regexp-prototype-get-matchAll-throws.js")]
    public Task regexp_prototype_get_matchAll_throws() => ExecutionTestFromFile("regexp-prototype-get-matchAll-throws");
    [Fact(DisplayName = "this-val-non-obj-coercible.js")]
    public Task this_val_non_obj_coercible() => ExecutionTestFromFile("this-val-non-obj-coercible");
}
