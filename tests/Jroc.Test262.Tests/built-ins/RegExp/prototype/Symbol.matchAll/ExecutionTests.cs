using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.RegExp.prototype.Symbol.matchAll;

public partial class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.RegExp.prototype.Symbol.matchAll") { }

    [Fact(DisplayName = "species-constructor.js")]
    public Task species_constructor()
        => ExecutionTestFromFile("species-constructor");

    [Fact(DisplayName = "species-constructor-species-is-null-or-undefined.js")]
    public Task species_constructor_species_is_null_or_undefined()
        => ExecutionTestFromFile("species-constructor-species-is-null-or-undefined");

    [Fact(DisplayName = "species-regexp-get-global-throws.js")]
    public Task species_regexp_get_global_throws()
        => ExecutionTestFromFile("species-regexp-get-global-throws");

    [Fact(DisplayName = "this-lastindex-cached.js")]
    public Task this_lastindex_cached()
        => ExecutionTestFromFile("this-lastindex-cached");

    [Fact(DisplayName = "this-tostring-flags.js")]
    public Task this_tostring_flags()
        => ExecutionTestFromFile("this-tostring-flags");

    [Fact(DisplayName = "isregexp-called-once.js")]
    public Task ported_isregexp_called_once() => ExecutionTestFromFile("isregexp-called-once");

    [Fact(DisplayName = "isregexp-this-throws.js")]
    public Task ported_isregexp_this_throws() => ExecutionTestFromFile("isregexp-this-throws");

    [Fact(DisplayName = "name.js")]
    public Task ported_name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "regexpcreate-this-throws.js")]
    public Task ported_regexpcreate_this_throws() => ExecutionTestFromFile("regexpcreate-this-throws");

    [Fact(DisplayName = "species-constructor-is-not-object-throws.js")]
    public Task ported_species_constructor_is_not_object_throws() => ExecutionTestFromFile("species-constructor-is-not-object-throws");
}
