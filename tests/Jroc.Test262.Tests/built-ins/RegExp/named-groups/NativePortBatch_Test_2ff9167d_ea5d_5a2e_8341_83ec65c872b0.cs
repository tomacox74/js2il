using Jroc.Test262.Tests;

namespace Jroc.Test262.Tests.built_ins.RegExp.named_groups;

public sealed class NativePortBatch_Test_2ff9167d_ea5d_5a2e_8341_83ec65c872b0 : DiskExecutionTestsBase
{
    public NativePortBatch_Test_2ff9167d_ea5d_5a2e_8341_83ec65c872b0() : base("Jroc.Test262.Tests.built_ins.RegExp.named_groups") { }

    [Fact(DisplayName = "string-replace-get")]
    public Task string_replace_get() => ExecutionTestFromFile("string-replace-get");

    [Fact(DisplayName = "string-replace-missing")]
    public Task string_replace_missing() => ExecutionTestFromFile("string-replace-missing");

    [Fact(DisplayName = "string-replace-undefined")]
    public Task string_replace_undefined() => ExecutionTestFromFile("string-replace-undefined");

}
