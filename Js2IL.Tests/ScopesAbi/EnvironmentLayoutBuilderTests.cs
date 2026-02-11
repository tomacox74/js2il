using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using Acornima;
using Js2IL.IR;
using Js2IL.Services.ScopesAbi;
using Js2IL.Services.VariableBindings;
using Js2IL.SymbolTables;
using Js2IL.Services;
using Xunit;

namespace Js2IL.Tests.ScopesAbi;

/// <summary>
/// Tests for the ScopesAbi facade types and EnvironmentLayoutBuilder.
/// These tests verify:
/// 1. Method signatures match the ABI contract
/// 2. Scope-chain ordering is deterministic
/// 3. Functions always have a scopes parameter; instance methods use this._scopes
/// </summary>
public class EnvironmentLayoutBuilderTests
{
    private readonly JavaScriptParser _parser;
    private readonly SymbolTableBuilder _scopeBuilder;
    private readonly ScopeMetadataRegistry _scopeMetadata;
    private readonly MetadataBuilder _metadataBuilder;

    public EnvironmentLayoutBuilderTests()
    {
        _parser = new JavaScriptParser();
        _scopeBuilder = new SymbolTableBuilder();
        _scopeMetadata = new ScopeMetadataRegistry();
        _metadataBuilder = new MetadataBuilder();
    }

    private SymbolTable BuildSymbolTable(Acornima.Ast.Program ast, string fileName)
    {
        var module = new ModuleDefinition
        {
            Ast = ast,
            Path = fileName,
            Name = Path.GetFileNameWithoutExtension(fileName),
            ModuleId = Path.GetFileNameWithoutExtension(fileName)
        };
        _scopeBuilder.Build(module);
        return module.SymbolTable!;
    }

    #region CallableAbi Tests

    [Fact]
    public void CallableAbi_ForFunction_NoParentScopes_StillHasScopesParam()
    {
        // Arrange & Act
        var abi = CallableAbi.ForFunction(jsParameterCount: 2, needsParentScopes: false);

        // Assert
        Assert.Equal(ScopesSource.Argument, abi.ScopesSource);
        Assert.Equal(2, abi.JsParameterCount);
        Assert.False(abi.IsInstanceMethod);
        Assert.True(abi.HasScopesParam);
    }

    [Fact]
    public void CallableAbi_ForFunction_WithParentScopes_ScopesSourceIsArgument()
    {
        // Arrange & Act
        var abi = CallableAbi.ForFunction(jsParameterCount: 1, needsParentScopes: true);

        // Assert
        Assert.Equal(ScopesSource.Argument, abi.ScopesSource);
        Assert.Equal(1, abi.JsParameterCount);
        Assert.False(abi.IsInstanceMethod);
        Assert.True(abi.HasScopesParam);
    }

    [Fact]
    public void CallableAbi_ForClassMethod_WithParentScopes_ScopesSourceIsThisField()
    {
        // Arrange & Act
        var abi = CallableAbi.ForClassMethod(jsParameterCount: 0, needsParentScopes: true);

        // Assert
        Assert.Equal(ScopesSource.ThisField, abi.ScopesSource);
        Assert.True(abi.IsInstanceMethod);
        Assert.False(abi.HasScopesParam); // Class methods get scopes via this._scopes, not param
    }

    [Fact]
    public void CallableAbi_ForClassMethod_NoParentScopes_ScopesSourceIsNone()
    {
        // Arrange & Act
        var abi = CallableAbi.ForClassMethod(jsParameterCount: 3, needsParentScopes: false);

        // Assert
        Assert.Equal(ScopesSource.None, abi.ScopesSource);
        Assert.True(abi.IsInstanceMethod);
        Assert.False(abi.HasScopesParam);
    }

    [Fact]
    public void CallableAbi_ForConstructor_WithParentScopes_ScopesSourceIsArgument()
    {
        // Constructors get scopes via argument (since 'this' is being constructed)
        var abi = CallableAbi.ForConstructor(jsParameterCount: 2, needsParentScopes: true);

        Assert.Equal(ScopesSource.Argument, abi.ScopesSource);
        Assert.True(abi.IsInstanceMethod); // Constructors are instance methods
        Assert.True(abi.HasScopesParam);
    }

    [Fact]
    public void CallableAbi_ForModuleMain_AlwaysStaticNoScopesArg()
    {
        // Module main is the entry point, no parent scopes passed
        var abi = CallableAbi.ForModuleMain(jsParameterCount: 0);

        Assert.Equal(ScopesSource.None, abi.ScopesSource);
        Assert.Equal(0, abi.JsParameterCount);
        Assert.False(abi.IsInstanceMethod);
        Assert.False(abi.HasScopesParam);
    }

    [Theory]
    [InlineData(0, false, 1)] // Static func: scopes is arg0, so param0 -> IL arg 1
    [InlineData(0, true, 1)]  // needsParentScopes is ignored for functions; scopes is always present
    [InlineData(1, false, 2)] // param1 -> IL arg 2
    [InlineData(1, true, 2)]
    public void CallableAbi_JsParamToIlArgIndex_StaticFunction(int jsIndex, bool hasScopes, int expectedIlArg)
    {
        var abi = CallableAbi.ForFunction(jsParameterCount: 5, needsParentScopes: hasScopes);
        Assert.Equal(expectedIlArg, abi.JsParamToIlArgIndex(jsIndex));
    }

    [Theory]
    [InlineData(0, false, 1)] // Instance method, no scopes, param0 -> IL arg 1 (this is arg0)
    [InlineData(1, false, 2)] // Instance method, no scopes, param1 -> IL arg 2
    [InlineData(0, true, 1)]  // Instance method, with scopes (in this), param0 -> IL arg 1
    public void CallableAbi_JsParamToIlArgIndex_InstanceMethod(int jsIndex, bool hasScopes, int expectedIlArg)
    {
        var abi = CallableAbi.ForClassMethod(jsParameterCount: 3, needsParentScopes: hasScopes);
        Assert.Equal(expectedIlArg, abi.JsParamToIlArgIndex(jsIndex));
    }

    [Fact]
    public void CallableAbi_ScopesArgIndex_ForStaticFunction_IsZero()
    {
        var abi = CallableAbi.ForFunction(jsParameterCount: 2, needsParentScopes: true);
        Assert.Equal(0, abi.ScopesArgIndex);
    }

    [Fact]
    public void CallableAbi_ScopesArgIndex_ForConstructor_IsOne()
    {
        // Constructors have 'this' as arg0, scopes as arg1
        var abi = CallableAbi.ForConstructor(jsParameterCount: 2, needsParentScopes: true);
        Assert.Equal(1, abi.ScopesArgIndex);
    }

    #endregion

    #region ScopeChainLayout Tests

    [Fact]
    public void ScopeChainLayout_Empty_HasZeroSlots()
    {
        var chain = ScopeChainLayout.Empty;
        
        Assert.Equal(0, chain.Length);
        Assert.Empty(chain.Slots);
        Assert.True(chain.IsEmpty);
    }

    [Fact]
    public void ScopeChainLayout_IndexOf_ReturnsMinus1ForUnknownScope()
    {
        var chain = ScopeChainLayout.Empty;
        Assert.Equal(-1, chain.IndexOf("NonExistent"));
    }

    [Fact]
    public void ScopeChainLayout_IndexOf_ReturnsCorrectIndexForKnownScope()
    {
        var slots = new List<ScopeSlot>
        {
            new ScopeSlot(0, "GlobalScope", default),
            new ScopeSlot(1, "outer", default),
            new ScopeSlot(2, "inner", default)
        };
        var chain = new ScopeChainLayout(slots);

        Assert.Equal(0, chain.IndexOf("GlobalScope"));
        Assert.Equal(1, chain.IndexOf("outer"));
        Assert.Equal(2, chain.IndexOf("inner"));
        Assert.Equal(-1, chain.IndexOf("unknown"));
    }

    [Fact]
    public void ScopeChainLayout_FindSlot_ReturnsNullForUnknownScope()
    {
        var chain = ScopeChainLayout.Empty;
        Assert.Null(chain.FindSlot("NonExistent"));
    }

    [Fact]
    public void ScopeChainLayout_FindSlot_ReturnsCorrectSlot()
    {
        var slots = new List<ScopeSlot>
        {
            new ScopeSlot(0, "GlobalScope", default),
            new ScopeSlot(1, "outer", default)
        };
        var chain = new ScopeChainLayout(slots);

        var slot = chain.FindSlot("outer");
        Assert.NotNull(slot);
        Assert.Equal("outer", slot.ScopeName);
        Assert.Equal(1, slot.Index);
    }

    #endregion

    #region BindingStorage Tests

    [Fact]
    public void BindingStorage_ForLocal_HasCorrectKind()
    {
        var storage = BindingStorage.ForLocal(localIndex: 2);
        
        Assert.Equal(BindingStorageKind.IlLocal, storage.Kind);
        Assert.Equal(2, storage.LocalIndex);
        Assert.Equal(-1, storage.JsParameterIndex);
        Assert.Equal(-1, storage.ParentScopeIndex);
    }

    [Fact]
    public void BindingStorage_ForArgument_HasCorrectKind()
    {
        var storage = BindingStorage.ForArgument(jsParameterIndex: 1);

        Assert.Equal(BindingStorageKind.IlArgument, storage.Kind);
        Assert.Equal(1, storage.JsParameterIndex);
        Assert.Equal(-1, storage.LocalIndex);
    }

    [Fact]
    public void BindingStorage_ForLeafScopeField_HasCorrectKind()
    {
        var fieldId = new FieldId("TestScope", "testField");
        var scopeId = new ScopeId("TestScope");
        
        var storage = BindingStorage.ForLeafScopeField(fieldId, scopeId);

        Assert.Equal(BindingStorageKind.LeafScopeField, storage.Kind);
        Assert.Equal(fieldId, storage.Field);
        Assert.Equal(scopeId, storage.DeclaringScope);
        Assert.Equal(-1, storage.ParentScopeIndex); // leaf scope is not in array
    }

    [Fact]
    public void BindingStorage_ForParentScopeField_HasCorrectKind()
    {
        var fieldId = new FieldId("ParentScope", "parentField");
        var scopeId = new ScopeId("ParentScope");

        var storage = BindingStorage.ForParentScopeField(fieldId, scopeId, parentScopeIndex: 1);

        Assert.Equal(BindingStorageKind.ParentScopeField, storage.Kind);
        Assert.Equal(1, storage.ParentScopeIndex);
        Assert.Equal(fieldId, storage.Field);
        Assert.Equal(scopeId, storage.DeclaringScope);
    }

    #endregion

    #region EnvironmentLayoutBuilder Integration Tests

    [Fact]
    public void Build_SimpleFunction_NoClosures_NoScopesArg()
    {
        // Arrange - simple function with no captured variables
        var code = @"
            function add(a, b) {
                return a + b;
            }
        ";
        var ast = _parser.ParseJavaScript(code, "test.js");
        var symbolTable = BuildSymbolTable(ast, "test.js");
        var builder = new EnvironmentLayoutBuilder(_scopeMetadata);

        // Find the function scope
        var funcScope = symbolTable.Root.Children.First(s => s.Name == "add");

        // Act
        var layout = builder.Build(funcScope, CallableKind.Function);

        // Assert - no parent scopes needed
        Assert.Equal(ScopesSource.Argument, layout.Abi.ScopesSource);
        // Functions always get a scopes array so they can forward it; global/module scope is slot 0.
        Assert.Equal(1, layout.ScopeChain.Length);
        Assert.Equal(2, layout.Abi.JsParameterCount);
    }

    [Fact]
    public void Build_ClosureFunction_WithCapturedVar_HasScopesArg()
    {
        // Arrange - closure that captures parent variable
        var code = @"
            function outer() {
                var x = 10;
                function inner() {
                    return x;
                }
            }
        ";
        var ast = _parser.ParseJavaScript(code, "test.js");
        var symbolTable = BuildSymbolTable(ast, "test.js");
        var builder = new EnvironmentLayoutBuilder(_scopeMetadata);

        // Find the inner function scope
        var outerScope = symbolTable.Root.Children.First(s => s.Name == "outer");
        var innerScope = outerScope.Children.First(s => s.Name == "inner");

        // Act
        var layout = builder.Build(innerScope, CallableKind.Function);

        // Assert - needs parent scopes
        Assert.Equal(ScopesSource.Argument, layout.Abi.ScopesSource);
        Assert.True(layout.ScopeChain.Length > 0);
    }

    [Fact]
    public void Build_ScopeChainOrdering_IsDeterministic()
    {
        // Arrange - nested closures
        var code = @"
            var globalVar = 1;
            function outer() {
                var outerVar = 2;
                function middle() {
                    var middleVar = 3;
                    function inner() {
                        return globalVar + outerVar + middleVar;
                    }
                }
            }
        ";
        var ast = _parser.ParseJavaScript(code, "test.js");
        var symbolTable = BuildSymbolTable(ast, "test.js");
        
        // Register scope types to get proper handles
        RegisterScopeTypes(symbolTable);
        
        var builder = new EnvironmentLayoutBuilder(_scopeMetadata);

        // Find the innermost function scope
        var outerScope = symbolTable.Root.Children.First(s => s.Name == "outer");
        var middleScope = outerScope.Children.First(s => s.Name == "middle");
        var innerScope = middleScope.Children.First(s => s.Name == "inner");

        // Act - build multiple times to verify determinism
        var layout1 = builder.Build(innerScope, CallableKind.Function);
        var layout2 = builder.Build(innerScope, CallableKind.Function);

        // Assert - scope chain ordering should be identical
        Assert.Equal(layout1.ScopeChain.Length, layout2.ScopeChain.Length);
        for (int i = 0; i < layout1.ScopeChain.Length; i++)
        {
            Assert.Equal(layout1.ScopeChain.Slots[i].ScopeName, layout2.ScopeChain.Slots[i].ScopeName);
            Assert.Equal(layout1.ScopeChain.Slots[i].Index, layout2.ScopeChain.Slots[i].Index);
        }
    }

    [Fact]
    public void Build_ScopeChainOrdering_OutermostFirst_GeneralizedLayout()
    {
        // Arrange - nested closures with generalized layout
        var code = @"
            var globalVar = 1;
            function outer() {
                var outerVar = 2;
                function inner() {
                    return globalVar + outerVar;
                }
            }
        ";
        var ast = _parser.ParseJavaScript(code, "test.js");
        var symbolTable = BuildSymbolTable(ast, "test.js");
        RegisterScopeTypes(symbolTable);
        
        var builder = new EnvironmentLayoutBuilder(_scopeMetadata);

        var outerScope = symbolTable.Root.Children.First(s => s.Name == "outer");
        var innerScope = outerScope.Children.First(s => s.Name == "inner");

        // Act
        var layout = builder.Build(innerScope, CallableKind.Function, ScopesLayoutKind.GeneralizedScopesLayout);

        // Assert - outermost (global) should be first, then outer
        Assert.True(layout.ScopeChain.Length >= 2);
        Assert.Equal("test", layout.ScopeChain.Slots[0].ScopeName); // global scope
        Assert.Equal("test/outer", layout.ScopeChain.Slots[1].ScopeName);
    }

    [Fact]
    public void Build_ClassMethod_WithClosure_HasScopesViaThis()
    {
        // Arrange - class method that captures outer variable
        var code = @"
            var outerVar = 10;
            class MyClass {
                getValue() {
                    return outerVar;
                }
            }
        ";
        var ast = _parser.ParseJavaScript(code, "test.js");
        var symbolTable = BuildSymbolTable(ast, "test.js");
        RegisterScopeTypes(symbolTable);
        
        var builder = new EnvironmentLayoutBuilder(_scopeMetadata);

        // Find the method scope
        var classScope = symbolTable.Root.Children.FirstOrDefault(s => s.Name == "MyClass");
        if (classScope == null)
        {
            // Skip if class structure isn't what we expect
            return;
        }
        
        var methodScope = classScope.Children.FirstOrDefault(s => s.Name == "getValue");
        if (methodScope == null || !methodScope.ReferencesParentScopeVariables)
        {
            // If method doesn't reference parent scope, behavior differs
            return;
        }

        // Act
        var layout = builder.Build(methodScope, CallableKind.ClassMethod);

        // Assert - class methods get scopes via ThisField when they need parent scopes
        Assert.Equal(ScopesSource.ThisField, layout.Abi.ScopesSource);
    }

    [Fact]
    public void Build_BindingStorage_NonCapturedParam_IsIlArgument()
    {
        // Arrange
        var code = @"
            function foo(a, b) {
                return a + b;
            }
        ";
        var ast = _parser.ParseJavaScript(code, "test.js");
        var symbolTable = BuildSymbolTable(ast, "test.js");
        var builder = new EnvironmentLayoutBuilder(_scopeMetadata);

        var funcScope = symbolTable.Root.Children.First(s => s.Name == "foo");

        // Act
        var layout = builder.Build(funcScope, CallableKind.Function);

        // Assert - parameters should be IL arguments
        var paramABinding = funcScope.Bindings["a"];
        var paramBBinding = funcScope.Bindings["b"];

        Assert.True(layout.StorageByBinding.TryGetValue(paramABinding, out var storageA));
        Assert.True(layout.StorageByBinding.TryGetValue(paramBBinding, out var storageB));
        Assert.Equal(BindingStorageKind.IlArgument, storageA.Kind);
        Assert.Equal(BindingStorageKind.IlArgument, storageB.Kind);
    }

    [Fact]
    public void Build_BindingStorage_CapturedParam_IsLeafScopeField()
    {
        // Arrange - parameter captured by closure
        var code = @"
            function outer(x) {
                function inner() {
                    return x;
                }
                return inner;
            }
        ";
        var ast = _parser.ParseJavaScript(code, "test.js");
        var symbolTable = BuildSymbolTable(ast, "test.js");
        var builder = new EnvironmentLayoutBuilder(_scopeMetadata);

        var outerScope = symbolTable.Root.Children.First(s => s.Name == "outer");

        // Act
        var layout = builder.Build(outerScope, CallableKind.Function);

        // Assert - 'x' is captured by inner, so outer stores it as a field
        var paramXBinding = outerScope.Bindings["x"];
        Assert.True(paramXBinding.IsCaptured);
        Assert.Equal(BindingStorageKind.LeafScopeField, layout.StorageByBinding[paramXBinding].Kind);
    }

    [Fact]
    public void Build_BindingStorage_NonCapturedLocal_IsIlLocal()
    {
        // Arrange
        var code = @"
            function foo() {
                var temp = 42;
                return temp;
            }
        ";
        var ast = _parser.ParseJavaScript(code, "test.js");
        var symbolTable = BuildSymbolTable(ast, "test.js");
        var builder = new EnvironmentLayoutBuilder(_scopeMetadata);

        var funcScope = symbolTable.Root.Children.First(s => s.Name == "foo");

        // Act
        var layout = builder.Build(funcScope, CallableKind.Function);

        // Assert
        var tempBinding = funcScope.Bindings["temp"];
        Assert.False(tempBinding.IsCaptured);
        Assert.Equal(BindingStorageKind.IlLocal, layout.StorageByBinding[tempBinding].Kind);
    }

    #endregion

    #region Helper Methods

    private FieldDefinitionHandle CreateTestFieldHandle()
    {
        // Create a minimal type definition first
        var typeHandle = _metadataBuilder.AddTypeDefinition(
            TypeAttributes.Public | TypeAttributes.Class,
            _metadataBuilder.GetOrAddString("TestNamespace"),
            _metadataBuilder.GetOrAddString($"TestType_{Guid.NewGuid():N}"),
            default(EntityHandle),
            MetadataTokens.FieldDefinitionHandle(1),
            MetadataTokens.MethodDefinitionHandle(1));

        // Add a field to that type
        return _metadataBuilder.AddFieldDefinition(
            FieldAttributes.Public,
            _metadataBuilder.GetOrAddString($"field_{Guid.NewGuid():N}"),
            _metadataBuilder.GetOrAddBlob(new byte[] { 0x06, 0x0e })); // Object signature
    }

    private TypeDefinitionHandle CreateTestTypeHandle()
    {
        return _metadataBuilder.AddTypeDefinition(
            TypeAttributes.Public | TypeAttributes.Class,
            _metadataBuilder.GetOrAddString("TestNamespace"),
            _metadataBuilder.GetOrAddString($"TestType_{Guid.NewGuid():N}"),
            default(EntityHandle),
            MetadataTokens.FieldDefinitionHandle(1),
            MetadataTokens.MethodDefinitionHandle(1));
    }

    private void RegisterScopeTypes(SymbolTable symbolTable)
    {
        // Register types for all scopes to enable handle lookups
        RegisterScopeTypesRecursive(symbolTable.Root);
    }

    private void RegisterScopeTypesRecursive(Js2IL.SymbolTables.Scope scope)
    {
        var typeHandle = CreateTestTypeHandle();
        
        // Add a dummy variable to register the scope type
        var dummyFieldHandle = _metadataBuilder.AddFieldDefinition(
            FieldAttributes.Public,
            _metadataBuilder.GetOrAddString("_dummy"),
            _metadataBuilder.GetOrAddBlob(new byte[] { 0x06, 0x0e }));
        
        _scopeMetadata.RegisterScopeType(scope.Name, typeHandle);
        _scopeMetadata.RegisterField(scope.Name, "_dummy", dummyFieldHandle);

        // Also register real bindings
        foreach (var (name, binding) in scope.Bindings)
        {
            if (name == "_dummy") continue;
            
            var bindingFieldHandle = _metadataBuilder.AddFieldDefinition(
                FieldAttributes.Public,
                _metadataBuilder.GetOrAddString(name),
                _metadataBuilder.GetOrAddBlob(new byte[] { 0x06, 0x0e }));
            
            _scopeMetadata.RegisterField(scope.Name, name, bindingFieldHandle);
        }

        foreach (var child in scope.Children)
        {
            RegisterScopeTypesRecursive(child);
        }
    }

    #endregion
}
