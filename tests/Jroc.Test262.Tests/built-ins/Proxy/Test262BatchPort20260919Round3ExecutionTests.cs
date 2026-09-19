using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Proxy;

public class Test262BatchPort20260919Round3ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort20260919Round3ExecutionTests() : base("built_ins.Proxy") { }

    [Fact(DisplayName = "proxy-undefined-newtarget")]
    public Task proxy_undefined_newtarget()
        => ExecutionTestFromFile("proxy-undefined-newtarget");

    [Fact(DisplayName = "proxy")]
    public Task proxy()
        => ExecutionTestFromFile("proxy");

}
