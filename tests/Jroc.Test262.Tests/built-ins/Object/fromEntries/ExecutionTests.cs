using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Object.fromEntries;

public class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Object.fromEntries") { }

    [Fact(DisplayName = "iterator-closed-for-throwing-entry-key-accessor")]
    public Task iterator_closed_for_throwing_entry_key_accessor()
        => ExecutionTestFromFile("iterator-closed-for-throwing-entry-key-accessor");

    [Fact(DisplayName = "iterator-closed-for-throwing-entry-key-tostring")]
    public Task iterator_closed_for_throwing_entry_key_tostring()
        => ExecutionTestFromFile("iterator-closed-for-throwing-entry-key-tostring");

    [Fact(DisplayName = "iterator-closed-for-throwing-entry-value-accessor")]
    public Task iterator_closed_for_throwing_entry_value_accessor()
        => ExecutionTestFromFile("iterator-closed-for-throwing-entry-value-accessor");

    [Fact(DisplayName = "iterator-not-closed-for-next-returning-non-object")]
    public Task iterator_not_closed_for_next_returning_non_object()
        => ExecutionTestFromFile("iterator-not-closed-for-next-returning-non-object");

    [Fact(DisplayName = "iterator-not-closed-for-throwing-done-accessor")]
    public Task iterator_not_closed_for_throwing_done_accessor()
        => ExecutionTestFromFile("iterator-not-closed-for-throwing-done-accessor");

    [Fact(DisplayName = "iterator-closed-for-string-entry")]
    public Task iterator_closed_for_string_entry()
        => ExecutionTestFromFile("iterator-closed-for-string-entry");

    [Fact(DisplayName = "string-entry-primitive-throws")]
    public Task string_entry_primitive_throws()
        => ExecutionTestFromFile("string-entry-primitive-throws");

    [Fact(DisplayName = "empty-iterable.js")]
    public Task Port200_empty_iterable()
        => ExecutionTestFromFile("empty-iterable");

    [Fact(DisplayName = "evaluation-order.js")]
    public Task Port200_evaluation_order()
        => ExecutionTestFromFile("evaluation-order");

    [Fact(DisplayName = "iterator-closed-for-null-entry.js")]
    public Task Port200_iterator_closed_for_null_entry()
        => ExecutionTestFromFile("iterator-closed-for-null-entry");

    [Fact(DisplayName = "iterator-not-closed-for-throwing-next.js")]
    public Task Port200_iterator_not_closed_for_throwing_next()
        => ExecutionTestFromFile("iterator-not-closed-for-throwing-next");

    [Fact(DisplayName = "iterator-not-closed-for-uncallable-next.js")]
    public Task Port200_iterator_not_closed_for_uncallable_next()
        => ExecutionTestFromFile("iterator-not-closed-for-uncallable-next");

    [Fact(DisplayName = "key-order.js")]
    public Task Port200_key_order()
        => ExecutionTestFromFile("key-order");

    [Fact(DisplayName = "length.js")]
    public Task Port200_length()
        => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name.js")]
    public Task Port200_name()
        => ExecutionTestFromFile("name");

    [Fact(DisplayName = "not-a-constructor.js")]
    public Task Port200_not_a_constructor()
        => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "prototype.js")]
    public Task Port200_prototype()
        => ExecutionTestFromFile("prototype");

    [Fact(DisplayName = "requires-argument.js")]
    public Task Port200_requires_argument()
        => ExecutionTestFromFile("requires-argument");

    [Fact(DisplayName = "simple-properties.js")]
    public Task Port200_simple_properties()
        => ExecutionTestFromFile("simple-properties");

    [Fact(DisplayName = "string-entry-object-succeeds.js")]
    public Task Port200_string_entry_object_succeeds()
        => ExecutionTestFromFile("string-entry-object-succeeds");

    [Fact(DisplayName = "string-entry-string-object-succeeds.js")]
    public Task Port200_string_entry_string_object_succeeds()
        => ExecutionTestFromFile("string-entry-string-object-succeeds");

    [Fact(DisplayName = "supports-symbols.js")]
    public Task Port200_supports_symbols()
        => ExecutionTestFromFile("supports-symbols");

    [Fact(DisplayName = "to-property-key.js")]
    public Task Port200_to_property_key()
        => ExecutionTestFromFile("to-property-key");

    [Fact(DisplayName = "uses-define-semantics.js")]
    public Task Port200_uses_define_semantics()
        => ExecutionTestFromFile("uses-define-semantics");

    [Fact(DisplayName = "uses-keys-not-iterator.js")]
    public Task Port200_uses_keys_not_iterator()
        => ExecutionTestFromFile("uses-keys-not-iterator");
}
