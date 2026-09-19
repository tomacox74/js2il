using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArrayConstructors.internals.OwnPropertyKeys;

public class Test262BatchPort20260919ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort20260919ExecutionTests() : base("built_ins.TypedArrayConstructors.internals.OwnPropertyKeys") { }

    [Fact(DisplayName = "integer-indexes-and-string-and-symbol-keys-")]
    public Task integer_indexes_and_string_and_symbol_keys_()
        => ExecutionTestFromFile("integer-indexes-and-string-and-symbol-keys-");

    [Fact(DisplayName = "integer-indexes-and-string-keys")]
    public Task integer_indexes_and_string_keys()
        => ExecutionTestFromFile("integer-indexes-and-string-keys");

    [Fact(DisplayName = "integer-indexes")]
    public Task integer_indexes()
        => ExecutionTestFromFile("integer-indexes");

}
