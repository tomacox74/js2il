using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Date.prototype.Symbol.toPrimitive;

public partial class ExecutionTests
{
    [Fact(DisplayName = "hint-number-first-valid.js")]
    public Task hint_number_first_valid() => ExecutionTestFromFile("hint-number-first-valid");

    [Fact(DisplayName = "hint-number-no-callables.js")]
    public Task hint_number_no_callables() => ExecutionTestFromFile("hint-number-no-callables");

    [Fact(DisplayName = "hint-string-first-invalid.js")]
    public Task hint_string_first_invalid() => ExecutionTestFromFile("hint-string-first-invalid");

    [Fact(DisplayName = "hint-string-first-non-callable.js")]
    public Task hint_string_first_non_callable() => ExecutionTestFromFile("hint-string-first-non-callable");

    [Fact(DisplayName = "hint-string-first-valid.js")]
    public Task hint_string_first_valid() => ExecutionTestFromFile("hint-string-first-valid");

    [Fact(DisplayName = "hint-string-no-callables.js")]
    public Task hint_string_no_callables() => ExecutionTestFromFile("hint-string-no-callables");

    [Fact(DisplayName = "length.js")]
    public Task length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name.js")]
    public Task name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "prop-desc.js")]
    public Task prop_desc() => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "this-val-non-obj.js")]
    public Task this_val_non_obj() => ExecutionTestFromFile("this-val-non-obj");

}
