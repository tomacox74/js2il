using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArrayConstructors.ctors.object_arg;

public class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.TypedArrayConstructors.ctors.object_arg") { }

    [Fact(DisplayName = "iterated-array-with-modified-array-iterator.js")]
    public Task Port400_iterated_array_with_modified_array_iterator()
        => ExecutionTestFromFile("iterated-array-with-modified-array-iterator");

    [Fact(DisplayName = "iterator-is-null-as-array-like.js")]
    public Task Port400_iterator_is_null_as_array_like()
        => ExecutionTestFromFile("iterator-is-null-as-array-like");

    [Fact(DisplayName = "length-excessive-throws.js")]
    public Task Port400_length_excessive_throws()
        => ExecutionTestFromFile("length-excessive-throws");

    [Fact(DisplayName = "undefined-newtarget-throws.js")]
    public Task Port400_undefined_newtarget_throws()
        => ExecutionTestFromFile("undefined-newtarget-throws");
}
