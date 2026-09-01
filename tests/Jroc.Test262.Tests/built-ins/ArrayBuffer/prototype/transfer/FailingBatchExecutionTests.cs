using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.ArrayBuffer.prototype.transfer;

public class FailingBatchExecutionTests : DiskExecutionTestsBase
{
    public FailingBatchExecutionTests() : base("built_ins.ArrayBuffer.prototype.transfer") { }

    [Fact(DisplayName = "descriptor.js")]
    public Task descriptor() => ExecutionTestFromFile("descriptor");

    [Fact(DisplayName = "extensible.js")]
    public Task extensible() => ExecutionTestFromFile("extensible");

    [Fact(DisplayName = "from-fixed-to-larger-no-resizable.js")]
    public Task from_fixed_to_larger_no_resizable() => ExecutionTestFromFile("from-fixed-to-larger-no-resizable");

    [Fact(DisplayName = "from-fixed-to-larger.js")]
    public Task from_fixed_to_larger() => ExecutionTestFromFile("from-fixed-to-larger");

    [Fact(DisplayName = "from-fixed-to-same-no-resizable.js")]
    public Task from_fixed_to_same_no_resizable() => ExecutionTestFromFile("from-fixed-to-same-no-resizable");

    [Fact(DisplayName = "from-fixed-to-same.js")]
    public Task from_fixed_to_same() => ExecutionTestFromFile("from-fixed-to-same");

    [Fact(DisplayName = "from-fixed-to-smaller-no-resizable.js")]
    public Task from_fixed_to_smaller_no_resizable() => ExecutionTestFromFile("from-fixed-to-smaller-no-resizable");

    [Fact(DisplayName = "from-fixed-to-smaller.js")]
    public Task from_fixed_to_smaller() => ExecutionTestFromFile("from-fixed-to-smaller");

    [Fact(DisplayName = "from-fixed-to-zero-no-resizable.js")]
    public Task from_fixed_to_zero_no_resizable() => ExecutionTestFromFile("from-fixed-to-zero-no-resizable");

    [Fact(DisplayName = "from-fixed-to-zero.js")]
    public Task from_fixed_to_zero() => ExecutionTestFromFile("from-fixed-to-zero");

    [Fact(DisplayName = "from-resizable-to-larger.js")]
    public Task from_resizable_to_larger() => ExecutionTestFromFile("from-resizable-to-larger");

    [Fact(DisplayName = "from-resizable-to-same.js")]
    public Task from_resizable_to_same() => ExecutionTestFromFile("from-resizable-to-same");

    [Fact(DisplayName = "from-resizable-to-smaller.js")]
    public Task from_resizable_to_smaller() => ExecutionTestFromFile("from-resizable-to-smaller");

    [Fact(DisplayName = "from-resizable-to-zero.js")]
    public Task from_resizable_to_zero() => ExecutionTestFromFile("from-resizable-to-zero");

    [Fact(DisplayName = "length.js")]
    public Task length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name.js")]
    public Task name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "new-length-excessive.js")]
    public Task new_length_excessive() => ExecutionTestFromFile("new-length-excessive");

    [Fact(DisplayName = "new-length-non-number.js")]
    public Task new_length_non_number() => ExecutionTestFromFile("new-length-non-number");

    [Fact(DisplayName = "nonconstructor.js")]
    public Task nonconstructor() => ExecutionTestFromFile("nonconstructor");

    [Fact(DisplayName = "this-is-detached.js")]
    public Task this_is_detached() => ExecutionTestFromFile("this-is-detached");

    [Fact(DisplayName = "this-is-not-arraybuffer-object.js")]
    public Task this_is_not_arraybuffer_object() => ExecutionTestFromFile("this-is-not-arraybuffer-object");

    [Fact(DisplayName = "this-is-not-object.js")]
    public Task this_is_not_object() => ExecutionTestFromFile("this-is-not-object");
}
