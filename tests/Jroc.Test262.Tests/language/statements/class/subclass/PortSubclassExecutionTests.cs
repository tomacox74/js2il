using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.statements.class_.subclass;

public class PortSubclassExecutionTests : DiskExecutionTestsBase
{
    public PortSubclassExecutionTests() : base("language.statements.class_.subclass") { }

    [Fact(DisplayName = "superclass-prototype-setter-method-override")]
    public Task superclass_prototype_setter_method_override()
        => ExecutionTest("superclass-prototype-setter-method-override");

    [Fact(DisplayName = "superclass-prototype-setter-constructor")]
    public Task superclass_prototype_setter_constructor()
        => ExecutionTest("superclass-prototype-setter-constructor");
}
