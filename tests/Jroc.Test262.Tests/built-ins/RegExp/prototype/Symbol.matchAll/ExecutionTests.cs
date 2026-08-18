using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.RegExp.prototype.Symbol.matchAll;

public class ExecutionTests : DiskExecutionTestsBase
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
}
