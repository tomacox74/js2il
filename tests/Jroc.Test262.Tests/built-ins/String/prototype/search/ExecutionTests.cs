using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.String.prototype.search;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.String.prototype.search") { }

    [Fact(DisplayName = "S15.5.4.12_A1_T1")]
    public Task S15_5_4_12_A1_T1()
        => ExecutionTestFromFile("S15.5.4.12_A1_T1");

    [Fact(DisplayName = "S15.5.4.12_A2_T1")]
    public Task S15_5_4_12_A2_T1()
        => ExecutionTestFromFile("S15.5.4.12_A2_T1");

    [Fact(DisplayName = "S15.5.4.12_A2_T3")]
    public Task S15_5_4_12_A2_T3()
        => ExecutionTestFromFile("S15.5.4.12_A2_T3");

    [Fact(DisplayName = "S15.5.4.12_A3_T2")]
    public Task S15_5_4_12_A3_T2()
        => ExecutionTestFromFile("S15.5.4.12_A3_T2");
    [Fact(DisplayName = "S15.5.4.12_A2_T4")]
    public Task S15_5_4_12_A2_T4()
        => ExecutionTestFromFile("S15.5.4.12_A2_T4");
    [Fact(DisplayName = "S15.5.4.12_A11")]
    public Task S15_5_4_12_A11()
        => ExecutionTestFromFile("S15.5.4.12_A11");
    [Fact(DisplayName = "S15.5.4.12_A1_T10")]
    public Task S15_5_4_12_A1_T10()
        => ExecutionTestFromFile("S15.5.4.12_A1_T10");
    [Fact(DisplayName = "S15.5.4.12_A2_T2")]
    public Task S15_5_4_12_A2_T2()
        => ExecutionTestFromFile("S15.5.4.12_A2_T2");

    [Fact(DisplayName = "cstm-search-on-bigint-primitive.js")]
    public Task cstm_search_on_bigint_primitive()
        => ExecutionTestFromFile("cstm-search-on-bigint-primitive");

    [Fact(DisplayName = "cstm-search-on-boolean-primitive.js")]
    public Task cstm_search_on_boolean_primitive()
        => ExecutionTestFromFile("cstm-search-on-boolean-primitive");

    [Fact(DisplayName = "cstm-search-on-number-primitive.js")]
    public Task cstm_search_on_number_primitive()
        => ExecutionTestFromFile("cstm-search-on-number-primitive");

    [Fact(DisplayName = "cstm-search-on-string-primitive.js")]
    public Task cstm_search_on_string_primitive()
        => ExecutionTestFromFile("cstm-search-on-string-primitive");

    [Fact(DisplayName = "invoke-builtin-search.js")]
    public Task invoke_builtin_search()
        => ExecutionTestFromFile("invoke-builtin-search");

    [Fact(DisplayName = "invoke-builtin-search-searcher-undef.js")]
    public Task invoke_builtin_search_searcher_undef()
        => ExecutionTestFromFile("invoke-builtin-search-searcher-undef");
}
