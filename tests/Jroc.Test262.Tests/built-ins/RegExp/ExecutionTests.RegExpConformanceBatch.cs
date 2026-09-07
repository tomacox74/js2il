using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.RegExp;

public partial class ExecutionTests
{
    [Fact(DisplayName = "regexp-class-chars.js")]
    public Task regexp_class_chars()
        => ExecutionTestFromFile("regexp-class-chars");

    [Fact(DisplayName = "syntax-err-arithmetic-modifiers-other-code-point-arbitrary.js")]
    public Task syntax_err_arithmetic_modifiers_other_code_point_arbitrary()
        => ExecutionTestFromFile("syntax-err-arithmetic-modifiers-other-code-point-arbitrary");

    [Fact(DisplayName = "syntax-err-arithmetic-modifiers-other-code-point-combining-i.js")]
    public Task syntax_err_arithmetic_modifiers_other_code_point_combining_i()
        => ExecutionTestFromFile("syntax-err-arithmetic-modifiers-other-code-point-combining-i");

    [Fact(DisplayName = "syntax-err-arithmetic-modifiers-other-code-point-combining-m.js")]
    public Task syntax_err_arithmetic_modifiers_other_code_point_combining_m()
        => ExecutionTestFromFile("syntax-err-arithmetic-modifiers-other-code-point-combining-m");

    [Fact(DisplayName = "syntax-err-arithmetic-modifiers-other-code-point-combining-s.js")]
    public Task syntax_err_arithmetic_modifiers_other_code_point_combining_s()
        => ExecutionTestFromFile("syntax-err-arithmetic-modifiers-other-code-point-combining-s");

    [Fact(DisplayName = "syntax-err-arithmetic-modifiers-other-code-point-d.js")]
    public Task syntax_err_arithmetic_modifiers_other_code_point_d()
        => ExecutionTestFromFile("syntax-err-arithmetic-modifiers-other-code-point-d");

}
