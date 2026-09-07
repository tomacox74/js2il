using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.RegExp.property_escapes;

public partial class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.RegExp.property_escapes") { }

    [Fact(DisplayName = "binary-property-with-value-ASCII_-_F-negated.js")]
    public Task binary_property_with_value_ASCII___F_negated()
        => CompilationFailureTest("binary-property-with-value-ASCII_-_F-negated");

    [Fact(DisplayName = "binary-property-with-value-ASCII_-_F.js")]
    public Task binary_property_with_value_ASCII___F()
        => CompilationFailureTest("binary-property-with-value-ASCII_-_F");

    [Fact(DisplayName = "binary-property-with-value-ASCII_-_Invalid-negated.js")]
    public Task binary_property_with_value_ASCII___Invalid_negated()
        => CompilationFailureTest("binary-property-with-value-ASCII_-_Invalid-negated");

    [Fact(DisplayName = "binary-property-with-value-ASCII_-_Invalid.js")]
    public Task binary_property_with_value_ASCII___Invalid()
        => CompilationFailureTest("binary-property-with-value-ASCII_-_Invalid");

    [Fact(DisplayName = "binary-property-with-value-ASCII_-_N-negated.js")]
    public Task binary_property_with_value_ASCII___N_negated()
        => CompilationFailureTest("binary-property-with-value-ASCII_-_N-negated");

    [Fact(DisplayName = "binary-property-with-value-ASCII_-_N.js")]
    public Task binary_property_with_value_ASCII___N()
        => CompilationFailureTest("binary-property-with-value-ASCII_-_N");

    [Fact(DisplayName = "binary-property-with-value-ASCII_-_No-negated.js")]
    public Task binary_property_with_value_ASCII___No_negated()
        => CompilationFailureTest("binary-property-with-value-ASCII_-_No-negated");

    [Fact(DisplayName = "binary-property-with-value-ASCII_-_No.js")]
    public Task binary_property_with_value_ASCII___No()
        => CompilationFailureTest("binary-property-with-value-ASCII_-_No");

    [Fact(DisplayName = "binary-property-with-value-ASCII_-_T-negated.js")]
    public Task binary_property_with_value_ASCII___T_negated()
        => CompilationFailureTest("binary-property-with-value-ASCII_-_T-negated");

    [Fact(DisplayName = "binary-property-with-value-ASCII_-_T.js")]
    public Task binary_property_with_value_ASCII___T()
        => CompilationFailureTest("binary-property-with-value-ASCII_-_T");

    [Fact(DisplayName = "binary-property-with-value-ASCII_-_Y-negated.js")]
    public Task binary_property_with_value_ASCII___Y_negated()
        => CompilationFailureTest("binary-property-with-value-ASCII_-_Y-negated");

    [Fact(DisplayName = "binary-property-with-value-ASCII_-_Y.js")]
    public Task binary_property_with_value_ASCII___Y()
        => CompilationFailureTest("binary-property-with-value-ASCII_-_Y");

    [Fact(DisplayName = "binary-property-with-value-ASCII_-_Yes-negated.js")]
    public Task binary_property_with_value_ASCII___Yes_negated()
        => CompilationFailureTest("binary-property-with-value-ASCII_-_Yes-negated");

    [Fact(DisplayName = "binary-property-with-value-ASCII_-_Yes.js")]
    public Task binary_property_with_value_ASCII___Yes()
        => CompilationFailureTest("binary-property-with-value-ASCII_-_Yes");

    [Fact(DisplayName = "character-class-range-end.js")]
    public Task character_class_range_end()
        => CompilationFailureTest("character-class-range-end");

    [Fact(DisplayName = "character-class-range-no-dash-end.js")]
    public Task character_class_range_no_dash_end()
        => CompilationFailureTest("character-class-range-no-dash-end");

    [Fact(DisplayName = "character-class-range-no-dash-start.js")]
    public Task character_class_range_no_dash_start()
        => CompilationFailureTest("character-class-range-no-dash-start");

    [Fact(DisplayName = "character-class-range-start.js")]
    public Task character_class_range_start()
        => CompilationFailureTest("character-class-range-start");

}
