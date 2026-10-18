using Acornima;
using Acornima.Ast;
using Jroc.HIR;
using Jroc.IR;
using Jroc.Services;
using Jroc.Services.ScopesAbi;
using Jroc.SymbolTables;

namespace Jroc.Tests;

public sealed class HIRPrivateAssignmentTargetTests
{
    [Theory]
    [InlineData("field", "field", null)]
    [InlineData("setter", null, "setter")]
    [InlineData("getter", null, null)]
    [InlineData("method", null, null)]
    public void PrivateAssignmentTargetCarriesItsLexicallyResolvedStoreMetadata(
        string name, string? fieldName, string? setterName)
    {
        var (method, scope) = ParseMethod($$"""
            class C {
                #field;
                set #setter(value) {}
                get #getter() {}
                #method() {}
                assign(source) { [this.#{{name}}] = source; }
            }
            """);
        var assignment = Assert.IsType<HIRDestructuringAssignmentExpression>(
            Assert.Single(method.Body.Statements.OfType<HIRExpressionStatement>()).Expression);
        var target = Assert.IsType<HIRPrivateTargetPattern>(
            Assert.Single(Assert.IsType<HIRArrayPattern>(assignment.Pattern).Elements));

        Assert.IsType<HIRThisExpression>(target.Receiver);
        Assert.Equal("Classes.private-assignment.C", target.RegistryClassName);
        Assert.Equal(fieldName, target.FieldName);
        Assert.Equal(
            setterName == null ? null : ClassElementNames.ManglePrivateAccessorMethodName("set", setterName),
            target.SetterMethodName);

        Assert.True(HIRToLIRLowerer.TryLower(
            method, scope, null, CallableKind.ClassMethod,
            hasScopesParameter: false, classRegistry: null, out var lir));
        if (fieldName != null)
        {
            Assert.Single(lir!.Instructions.OfType<LIRStorePrivateReceiverField>());
        }
        else if (setterName != null)
        {
            Assert.Contains(lir!.Instructions.OfType<LIRCallRuntimeServicesStatic>(),
                call => call.MethodName == nameof(JavaScriptRuntime.RuntimeServices.CallDirectClassPrivateMethod));
        }
        else
        {
            Assert.Single(lir!.Instructions.OfType<LIRThrowNewTypeError>());
        }
    }

    [Fact]
    public void ObjectRestResolvesItsPrivateReceiverBeforeCopyingProperties()
    {
        var (method, scope) = ParseMethod("""
            class C {
                #field;
                assign(source) { ({...this.#field} = source); }
            }
            """);
        Assert.True(HIRToLIRLowerer.TryLower(
            method, scope, null, CallableKind.ClassMethod,
            hasScopesParameter: false, classRegistry: null, out var lir));

        var instructions = lir!.Instructions;
        var store = Assert.Single(instructions.OfType<LIRStorePrivateReceiverField>());
        var rest = Assert.Single(instructions.OfType<LIRCallIntrinsicStatic>(),
            call => call.MethodName == nameof(JavaScriptRuntime.ObjectRuntime.Rest));
        var storeIndex = instructions.IndexOf(store);
        var restIndex = instructions.IndexOf(rest);
        Assert.True(restIndex < storeIndex);
        Assert.Contains(instructions.Take(restIndex),
            instruction => instruction is LIRLoadThis load && load.Result == store.Receiver);
        Assert.DoesNotContain(instructions.Skip(restIndex + 1).Take(storeIndex - restIndex - 1),
            instruction => instruction is LIRLoadThis);
    }

    private static (HIRMethod Method, Jroc.SymbolTables.Scope Scope) ParseMethod(string source)
    {
        var program = new JavaScriptParser().ParseJavaScript(source, "private-assignment.js");
        var declaration = Assert.IsType<ClassDeclaration>(Assert.Single(program.Body));
        var definition = Assert.Single(declaration.Body.Body.OfType<MethodDefinition>(),
            method => method.Key is Identifier { Name: "assign" });
        var module = new ModuleDefinition
        {
            Ast = program,
            Path = "private-assignment.js",
            Name = "private-assignment",
            ModuleId = "private-assignment"
        };
        new SymbolTableBuilder().Build(module);
        var scope = module.SymbolTable!.FindScopeByAstNode(definition.Value)!;
        Assert.True(HIRBuilder.TryParseMethod(
            definition, scope, CallableKind.ClassMethod,
            hasScopesParameter: false, out var method));
        return (method!, scope);
    }
}
