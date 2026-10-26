namespace Jroc.Test262.Tests.language.statements.Test_class.subclass.builtin_objects.Symbol;

public sealed class RemainingLanguageCompletionExecutionTests : DiskExecutionTestsBase
{
    public RemainingLanguageCompletionExecutionTests() : base("language.statements.Test_class.subclass.builtin_objects.Symbol") { }

    [Fact(DisplayName = "new-symbol-with-super-throws.js")]
    public Task new_symbol_with_super_throws()
        => ExecutionTest("new-symbol-with-super-throws");

    [Fact(DisplayName = "symbol-valid-as-extends-value.js")]
    public Task symbol_valid_as_extends_value()
        => ExecutionTest("symbol-valid-as-extends-value");
}
