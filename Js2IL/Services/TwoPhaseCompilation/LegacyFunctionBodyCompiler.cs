using System;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using Acornima.Ast;
using Js2IL.SymbolTables;

namespace Js2IL.Services.TwoPhaseCompilation;

internal static class LegacyFunctionBodyCompiler
{
    public static CompiledCallableBody CompileFunctionDeclarationBody(
        IServiceProvider serviceProvider,
        MetadataBuilder metadataBuilder,
        MethodBodyStreamEncoder methodBodyStreamEncoder,
        BaseClassLibraryReferences bclReferences,
        ClassRegistry classRegistry,
        SymbolTable symbolTable,
        CallableId callable,
        MethodDefinitionHandle expectedMethodDef,
        FunctionDeclaration functionDeclaration,
        Scope functionScope,
        string registryScopeName)
    {
        throw new NotSupportedException(
            "[TwoPhase] Legacy function body compilation has been removed. The two-phase pipeline requires IR compilation for function declarations.");
    }
}
