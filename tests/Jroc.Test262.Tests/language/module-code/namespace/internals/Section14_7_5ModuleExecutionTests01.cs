namespace Jroc.Test262.Tests.language.module_code.namespace_.internals;

public class Section14_7_5ModuleExecutionTests01 : Jroc.Test262.Tests.language.modules.FileSystemExecutionTestsBase
{
    public Section14_7_5ModuleExecutionTests01() : base("language/module-code/namespace/internals", "language.module_code.namespace_.internals") { }

    [Fact(DisplayName = "enumerate-binding-uninit.js")]
    public Task enumerate_binding_uninit_1()
        => ExecutionTest("enumerate-binding-uninit");
}
