using Jroc.Test262.Tests.language.modules;

namespace Jroc.Test262.Tests.language.future_reserved_words;

public class LexicalSourceConformanceBatchTests : FileSystemExecutionTestsBase
{
    public LexicalSourceConformanceBatchTests() : base("language/future-reserved-words", "language.future_reserved_words") { }

    [Fact(DisplayName = "_implements.js")]
    public Task _implements()
        => ExecutionTest("_implements");

    [Fact(DisplayName = "abstract.js")]
    public Task _abstract()
        => ExecutionTest("abstract");

    [Fact(DisplayName = "boolean.js")]
    public Task boolean()
        => ExecutionTest("boolean");

    [Fact(DisplayName = "byte.js")]
    public Task _byte()
        => ExecutionTest("byte");

    [Fact(DisplayName = "char.js")]
    public Task _char()
        => ExecutionTest("char");

    [Fact(DisplayName = "class.js")]
    public Task _class()
        => CompilationFailureTest("class");

    [Fact(DisplayName = "const.js")]
    public Task _const()
        => CompilationFailureTest("const");

    [Fact(DisplayName = "debugger.js")]
    public Task debugger()
        => CompilationFailureTest("debugger");

    [Fact(DisplayName = "double.js")]
    public Task _double()
        => ExecutionTest("double");

    [Fact(DisplayName = "enum.js")]
    public Task _enum()
        => CompilationFailureTest("enum");

    [Fact(DisplayName = "export.js")]
    public Task export()
        => CompilationFailureTest("export");

    [Fact(DisplayName = "extends.js")]
    public Task extends()
        => CompilationFailureTest("extends");

    [Fact(DisplayName = "final.js")]
    public Task final()
        => ExecutionTest("final");

    [Fact(DisplayName = "float.js")]
    public Task _float()
        => ExecutionTest("float");

    [Fact(DisplayName = "goto.js")]
    public Task _goto()
        => ExecutionTest("goto");

    [Fact(DisplayName = "implement.js")]
    public Task implement()
        => ExecutionTest("implement");

    [Fact(DisplayName = "implements-strict-escaped.js")]
    public Task implements_strict_escaped()
        => CompilationFailureTest("implements-strict-escaped");

    [Fact(DisplayName = "implements-strict.js")]
    public Task implements_strict()
        => CompilationFailureTest("implements-strict");

    [Fact(DisplayName = "implements-titlecase.js")]
    public Task implements_titlecase()
        => ExecutionTest("implements-titlecase");

    [Fact(DisplayName = "implements-uppercase.js")]
    public Task implements_uppercase()
        => ExecutionTest("implements-uppercase");

    [Fact(DisplayName = "implements.js")]
    public Task implements()
        => ExecutionTest("implements");

    [Fact(DisplayName = "implements0.js")]
    public Task implements0()
        => ExecutionTest("implements0");

    [Fact(DisplayName = "implementss.js")]
    public Task implementss()
        => ExecutionTest("implementss");

    [Fact(DisplayName = "import.js")]
    public Task import()
        => CompilationFailureTest("import");

    [Fact(DisplayName = "int.js")]
    public Task _int()
        => ExecutionTest("int");

    [Fact(DisplayName = "interface-strict-escaped.js")]
    public Task interface_strict_escaped()
        => CompilationFailureTest("interface-strict-escaped");

    [Fact(DisplayName = "interface-strict.js")]
    public Task interface_strict()
        => CompilationFailureTest("interface-strict");

    [Fact(DisplayName = "interface.js")]
    public Task _interface()
        => ExecutionTest("interface");

    [Fact(DisplayName = "let-strict-escaped.js")]
    public Task let_strict_escaped()
        => CompilationFailureTest("let-strict-escaped");

    [Fact(DisplayName = "let-strict.js")]
    public Task let_strict()
        => CompilationFailureTest("let-strict");

    [Fact(DisplayName = "long.js")]
    public Task _long()
        => ExecutionTest("long");

    [Fact(DisplayName = "native.js")]
    public Task native()
        => ExecutionTest("native");

    [Fact(DisplayName = "package-strict-escaped.js")]
    public Task package_strict_escaped()
        => CompilationFailureTest("package-strict-escaped");

    [Fact(DisplayName = "package-strict.js")]
    public Task package_strict()
        => CompilationFailureTest("package-strict");

    [Fact(DisplayName = "package.js")]
    public Task package()
        => ExecutionTest("package");

    [Fact(DisplayName = "private-strict-escaped.js")]
    public Task private_strict_escaped()
        => CompilationFailureTest("private-strict-escaped");

    [Fact(DisplayName = "private-strict.js")]
    public Task private_strict()
        => CompilationFailureTest("private-strict");

    [Fact(DisplayName = "private.js")]
    public Task _private()
        => ExecutionTest("private");

    [Fact(DisplayName = "protected-strict-escaped.js")]
    public Task protected_strict_escaped()
        => CompilationFailureTest("protected-strict-escaped");

    [Fact(DisplayName = "protected-strict.js")]
    public Task protected_strict()
        => CompilationFailureTest("protected-strict");

    [Fact(DisplayName = "protected.js")]
    public Task _protected()
        => ExecutionTest("protected");

    [Fact(DisplayName = "public-strict-escaped.js")]
    public Task public_strict_escaped()
        => CompilationFailureTest("public-strict-escaped");

    [Fact(DisplayName = "public-strict.js")]
    public Task public_strict()
        => CompilationFailureTest("public-strict");

    [Fact(DisplayName = "public.js")]
    public Task _public()
        => ExecutionTest("public");

    [Fact(DisplayName = "short.js")]
    public Task _short()
        => ExecutionTest("short");

    [Fact(DisplayName = "static-strict-escaped.js")]
    public Task static_strict_escaped()
        => CompilationFailureTest("static-strict-escaped");

    [Fact(DisplayName = "static-strict.js")]
    public Task static_strict()
        => CompilationFailureTest("static-strict");

    [Fact(DisplayName = "static.js")]
    public Task _static()
        => ExecutionTest("static");

    [Fact(DisplayName = "super.js")]
    public Task super()
        => CompilationFailureTest("super");

    [Fact(DisplayName = "synchronized.js")]
    public Task synchronized()
        => ExecutionTest("synchronized");

    [Fact(DisplayName = "throws.js")]
    public Task throws()
        => ExecutionTest("throws");

    [Fact(DisplayName = "transient.js")]
    public Task transient()
        => ExecutionTest("transient");

    [Fact(DisplayName = "volatile.js")]
    public Task _volatile()
        => ExecutionTest("volatile");

    [Fact(DisplayName = "yield-strict-escaped.js")]
    public Task yield_strict_escaped()
        => CompilationFailureTest("yield-strict-escaped");

    [Fact(DisplayName = "yield-strict.js")]
    public Task yield_strict()
        => CompilationFailureTest("yield-strict");
}
