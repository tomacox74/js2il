using Jroc.Test262.Tests.language.statements;

namespace Jroc.Test262.Tests.language.statements.class_.definition;

public class ParseExecutionTests : FileSystemExecutionTestsBase
{
    public ParseExecutionTests() : base(@"language\statements\class\definition", "language.statements.class_.definition") { }

    [Fact(DisplayName = "early-errors-class-method-body-contains-super-call")]
    public Task early_errors_class_method_body_contains_super_call()
        => CompilationFailureTest("early-errors-class-method-body-contains-super-call", "Failed to parse JavaScript");

    [Fact(DisplayName = "early-errors-class-method-formals-contains-super-call")]
    public Task early_errors_class_method_formals_contains_super_call()
        => CompilationFailureTest("early-errors-class-method-formals-contains-super-call", "Failed to parse JavaScript");

    [Fact(DisplayName = "early-errors-class-method-duplicate-parameters")]
    public Task early_errors_class_method_duplicate_parameters()
        => CompilationFailureTest("early-errors-class-method-duplicate-parameters", "Failed to parse JavaScript");

    [Fact(DisplayName = "early-errors-class-method-formals-body-duplicate")]
    public Task early_errors_class_method_formals_body_duplicate()
        => CompilationFailureTest("early-errors-class-method-formals-body-duplicate", "Failed to parse JavaScript");
}
