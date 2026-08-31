using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.String.prototype.replaceAll;

public partial class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.String.prototype.replaceAll") { }

    [Fact(DisplayName = "getSubstitution-0x0024-0x0024.js")]
    public Task getSubstitution_0x0024_0x0024()
        => ExecutionTestFromFile("getSubstitution-0x0024-0x0024");

    [Fact(DisplayName = "getSubstitution-0x0024-0x0026.js")]
    public Task getSubstitution_0x0024_0x0026()
        => ExecutionTestFromFile("getSubstitution-0x0024-0x0026");

    [Fact(DisplayName = "getSubstitution-0x0024-0x0027.js")]
    public Task getSubstitution_0x0024_0x0027()
        => ExecutionTestFromFile("getSubstitution-0x0024-0x0027");

    [Fact(DisplayName = "getSubstitution-0x0024-0x003C.js")]
    public Task getSubstitution_0x0024_0x003C()
        => ExecutionTestFromFile("getSubstitution-0x0024-0x003C");

    [Fact(DisplayName = "getSubstitution-0x0024-0x0060.js")]
    public Task getSubstitution_0x0024_0x0060()
        => ExecutionTestFromFile("getSubstitution-0x0024-0x0060");

    [Fact(DisplayName = "getSubstitution-0x0024.js")]
    public Task getSubstitution_0x0024()
        => ExecutionTestFromFile("getSubstitution-0x0024");

    [Fact(DisplayName = "getSubstitution-0x0024N.js")]
    public Task getSubstitution_0x0024N()
        => ExecutionTestFromFile("getSubstitution-0x0024N");

    [Fact(DisplayName = "getSubstitution-0x0024NN.js")]
    public Task getSubstitution_0x0024NN()
        => ExecutionTestFromFile("getSubstitution-0x0024NN");

    [Fact(DisplayName = "cstm-replaceall-on-bigint-primitive.js")]
    public Task cstm_replaceall_on_bigint_primitive()
        => ExecutionTestFromFile("cstm-replaceall-on-bigint-primitive");

    [Fact(DisplayName = "cstm-replaceall-on-boolean-primitive.js")]
    public Task cstm_replaceall_on_boolean_primitive()
        => ExecutionTestFromFile("cstm-replaceall-on-boolean-primitive");

    [Fact(DisplayName = "cstm-replaceall-on-number-primitive.js")]
    public Task cstm_replaceall_on_number_primitive()
        => ExecutionTestFromFile("cstm-replaceall-on-number-primitive");

    [Fact(DisplayName = "cstm-replaceall-on-string-primitive.js")]
    public Task cstm_replaceall_on_string_primitive()
        => ExecutionTestFromFile("cstm-replaceall-on-string-primitive");

    [Fact(DisplayName = "replaceValue-call-abrupt.js")]
    public Task replaceValue_call_abrupt()
        => ExecutionTestFromFile("replaceValue-call-abrupt");

    [Fact(DisplayName = "replaceValue-call-each-match-position.js")]
    public Task replaceValue_call_each_match_position()
        => ExecutionTestFromFile("replaceValue-call-each-match-position");

    [Fact(DisplayName = "replaceValue-call-matching-empty.js")]
    public Task replaceValue_call_matching_empty()
        => ExecutionTestFromFile("replaceValue-call-matching-empty");

    [Fact(DisplayName = "replaceValue-call-tostring-abrupt.js")]
    public Task replaceValue_call_tostring_abrupt()
        => ExecutionTestFromFile("replaceValue-call-tostring-abrupt");

    [Fact(DisplayName = "replaceValue-fn-skip-toString.js")]
    public Task replaceValue_fn_skip_toString()
        => ExecutionTestFromFile("replaceValue-fn-skip-toString");

    [Fact(DisplayName = "searchValue-flags-no-g-throws.js")]
    public Task searchValue_flags_no_g_throws()
        => ExecutionTestFromFile("searchValue-flags-no-g-throws");

    [Fact(DisplayName = "searchValue-flags-null-undefined-throws.js")]
    public Task searchValue_flags_null_undefined_throws()
        => ExecutionTestFromFile("searchValue-flags-null-undefined-throws");

    [Fact(DisplayName = "searchValue-flags-toString-abrupt.js")]
    public Task searchValue_flags_toString_abrupt()
        => ExecutionTestFromFile("searchValue-flags-toString-abrupt");

    [Fact(DisplayName = "searchValue-get-flags-abrupt.js")]
    public Task searchValue_get_flags_abrupt()
        => ExecutionTestFromFile("searchValue-get-flags-abrupt");

    [Fact(DisplayName = "searchValue-isRegExp-abrupt.js")]
    public Task searchValue_isRegExp_abrupt()
        => ExecutionTestFromFile("searchValue-isRegExp-abrupt");

    [Fact(DisplayName = "searchValue-replacer-before-tostring.js")]
    public Task searchValue_replacer_before_tostring()
        => ExecutionTestFromFile("searchValue-replacer-before-tostring");

    [Fact(DisplayName = "searchValue-replacer-call-abrupt.js")]
    public Task searchValue_replacer_call_abrupt()
        => ExecutionTestFromFile("searchValue-replacer-call-abrupt");

    [Fact(DisplayName = "searchValue-replacer-call.js")]
    public Task searchValue_replacer_call()
        => ExecutionTestFromFile("searchValue-replacer-call");

    [Fact(DisplayName = "searchValue-replacer-is-null.js")]
    public Task searchValue_replacer_is_null()
        => ExecutionTestFromFile("searchValue-replacer-is-null");

    [Fact(DisplayName = "searchValue-replacer-method-abrupt.js")]
    public Task searchValue_replacer_method_abrupt()
        => ExecutionTestFromFile("searchValue-replacer-method-abrupt");
}
