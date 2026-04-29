using Js2IL.Tests;

namespace Js2IL.Test262.Tests.language.literals.bigint.numeric_separators;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("language.literals.bigint.numeric_separators") { }

    [Fact(DisplayName = "numeric-separator-literal-bil-bd-nsl-bd")]
    public Task numeric_separator_literal_bil_bd_nsl_bd()
        => ExecutionTest("numeric-separator-literal-bil-bd-nsl-bd");

    [Fact(DisplayName = "numeric-separator-literal-bil-bd-nsl-bds")]
    public Task numeric_separator_literal_bil_bd_nsl_bds()
        => ExecutionTest("numeric-separator-literal-bil-bd-nsl-bds");

    [Fact(DisplayName = "numeric-separator-literal-bil-bds-nsl-bd")]
    public Task numeric_separator_literal_bil_bds_nsl_bd()
        => ExecutionTest("numeric-separator-literal-bil-bds-nsl-bd");

    [Fact(DisplayName = "numeric-separator-literal-bil-bds-nsl-bds")]
    public Task numeric_separator_literal_bil_bds_nsl_bds()
        => ExecutionTest("numeric-separator-literal-bil-bds-nsl-bds");

    [Fact(DisplayName = "numeric-separator-literal-dd-nsl-dd-one-of")]
    public Task numeric_separator_literal_dd_nsl_dd_one_of()
        => ExecutionTest("numeric-separator-literal-dd-nsl-dd-one-of");

    [Fact(DisplayName = "numeric-separator-literal-dds-nsl-dd")]
    public Task numeric_separator_literal_dds_nsl_dd()
        => ExecutionTest("numeric-separator-literal-dds-nsl-dd");
}
