using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.RegExp.prototype.Symbol.matchAll;

public partial class ExecutionTests
{
    [Fact(DisplayName = "length.js")]
    public Task length()
        => ExecutionTestFromFile("length");

    [Fact(DisplayName = "not-a-constructor.js")]
    public Task not_a_constructor()
        => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "prop-desc.js")]
    public Task prop_desc()
        => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "species-constructor-get-constructor-throws.js")]
    public Task species_constructor_get_constructor_throws()
        => ExecutionTestFromFile("species-constructor-get-constructor-throws");

    [Fact(DisplayName = "species-constructor-get-species-throws.js")]
    public Task species_constructor_get_species_throws()
        => ExecutionTestFromFile("species-constructor-get-species-throws");

    [Fact(DisplayName = "species-constructor-is-undefined.js")]
    public Task species_constructor_is_undefined()
        => ExecutionTestFromFile("species-constructor-is-undefined");

    [Fact(DisplayName = "species-constructor-species-is-not-constructor.js")]
    public Task species_constructor_species_is_not_constructor()
        => ExecutionTestFromFile("species-constructor-species-is-not-constructor");

    [Fact(DisplayName = "species-constructor-species-throws.js")]
    public Task species_constructor_species_throws()
        => ExecutionTestFromFile("species-constructor-species-throws");

    [Fact(DisplayName = "species-regexp-get-unicode-throws.js")]
    public Task species_regexp_get_unicode_throws()
        => ExecutionTestFromFile("species-regexp-get-unicode-throws");

    [Fact(DisplayName = "string-tostring-throws.js")]
    public Task string_tostring_throws()
        => ExecutionTestFromFile("string-tostring-throws");

    [Fact(DisplayName = "string-tostring.js")]
    public Task string_tostring()
        => ExecutionTestFromFile("string-tostring");

    [Fact(DisplayName = "this-get-flags-throws.js")]
    public Task this_get_flags_throws()
        => ExecutionTestFromFile("this-get-flags-throws");

    [Fact(DisplayName = "this-get-flags.js")]
    public Task this_get_flags()
        => ExecutionTestFromFile("this-get-flags");

    [Fact(DisplayName = "this-not-object-throws.js")]
    public Task this_not_object_throws()
        => ExecutionTestFromFile("this-not-object-throws");

    [Fact(DisplayName = "this-tolength-lastindex-throws.js")]
    public Task this_tolength_lastindex_throws()
        => ExecutionTestFromFile("this-tolength-lastindex-throws");

    [Fact(DisplayName = "this-tostring-flags-throws.js")]
    public Task this_tostring_flags_throws()
        => ExecutionTestFromFile("this-tostring-flags-throws");

}
