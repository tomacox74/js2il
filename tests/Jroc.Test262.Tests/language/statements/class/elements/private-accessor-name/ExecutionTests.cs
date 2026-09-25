using Jroc.Tests;

namespace Jroc.Test262.Tests.language.statements.class_.elements.private_accessor_name;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("language.statements.class_.elements.private-accessor-name") { }

    [Fact(DisplayName = "static-private-escape-sequence-ZWJ.js")]
    public Task ported_static_private_escape_sequence_ZWJ() => ExecutionTest("static-private-escape-sequence-ZWJ");

    [Fact(DisplayName = "static-private-escape-sequence-ZWNJ.js")]
    public Task ported_static_private_escape_sequence_ZWNJ() => ExecutionTest("static-private-escape-sequence-ZWNJ");

    [Fact(DisplayName = "static-private-escape-sequence-u2118.js")]
    public Task ported_static_private_escape_sequence_u2118() => ExecutionTest("static-private-escape-sequence-u2118");

    [Fact(DisplayName = "static-private-escape-sequence-u6F.js")]
    public Task ported_static_private_escape_sequence_u6F() => ExecutionTest("static-private-escape-sequence-u6F");

    [Fact(DisplayName = "static-private-name-ZWJ.js")]
    public Task ported_static_private_name_ZWJ() => ExecutionTest("static-private-name-ZWJ");

    [Fact(DisplayName = "static-private-name-ZWNJ.js")]
    public Task ported_static_private_name_ZWNJ() => ExecutionTest("static-private-name-ZWNJ");

    [Fact(DisplayName = "static-private-name-common.js")]
    public Task ported_static_private_name_common() => ExecutionTest("static-private-name-common");

    [Fact(DisplayName = "static-private-name-dollar.js")]
    public Task ported_static_private_name_dollar() => ExecutionTest("static-private-name-dollar");

    [Fact(DisplayName = "static-private-name-u2118.js")]
    public Task ported_static_private_name_u2118() => ExecutionTest("static-private-name-u2118");

    [Fact(DisplayName = "static-private-name-underscore.js")]
    public Task ported_static_private_name_underscore() => ExecutionTest("static-private-name-underscore");
}
