namespace Jroc.Test262.Tests.built_ins.Object.prototype.toString;

public class ObjectCompletionExecutionTests : InMemoryExecutionTestsBase
{
    public ObjectCompletionExecutionTests() : base("built_ins.Object.prototype.toString") { }

    [Fact(DisplayName = "name.js")]
    public Task name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "proxy-array.js")]
    public Task proxy_array() => ExecutionTestFromFile("proxy-array");

    [Fact(DisplayName = "proxy-revoked-during-get-call.js")]
    public Task proxy_revoked_during_get_call() => ExecutionTestFromFile("proxy-revoked-during-get-call");

    [Fact(DisplayName = "symbol-tag-generators-builtin.js")]
    public Task symbol_tag_generators_builtin() => ExecutionTestFromFile("symbol-tag-generators-builtin");

    [Fact(DisplayName = "symbol-tag-override-instances.js")]
    public Task symbol_tag_override_instances() => ExecutionTestFromFile("symbol-tag-override-instances");

    [Fact(DisplayName = "symbol-tag-override-primitives.js")]
    public Task symbol_tag_override_primitives() => ExecutionTestFromFile("symbol-tag-override-primitives");

    [Fact(DisplayName = "symbol-tag-string-builtin.js")]
    public Task symbol_tag_string_builtin() => ExecutionTestFromFile("symbol-tag-string-builtin");
}
