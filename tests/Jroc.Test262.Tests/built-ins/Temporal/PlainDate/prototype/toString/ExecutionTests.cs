using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Temporal.PlainDate.prototype.toString;

public sealed class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Temporal.PlainDate.prototype.toString") { }

    [Fact(DisplayName = "basic.js")]
    public Task Port300Nov19_basic() => ExecutionTestFromFile("basic");

    [Fact(DisplayName = "branding.js")]
    public Task Port300Nov19_branding() => ExecutionTestFromFile("branding");

    [Fact(DisplayName = "builtin.js")]
    public Task Port300Nov19_builtin() => ExecutionTestFromFile("builtin");

    [Fact(DisplayName = "calendarname-always.js")]
    public Task Port300Nov19_calendarname_always() => ExecutionTestFromFile("calendarname-always");

    [Fact(DisplayName = "calendarname-auto.js")]
    public Task Port300Nov19_calendarname_auto() => ExecutionTestFromFile("calendarname-auto");

    [Fact(DisplayName = "calendarname-critical.js")]
    public Task Port300Nov19_calendarname_critical() => ExecutionTestFromFile("calendarname-critical");
}
