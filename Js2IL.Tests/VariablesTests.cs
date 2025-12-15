using System;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using Js2IL.Services;
using Js2IL.Services.VariableBindings;
using Js2IL.SymbolTables;
using Xunit;

namespace Js2IL.Tests
{
    /// <summary>
    /// Tests for the Variables class, which manages variable resolution and local slot allocation
    /// during IL generation. These tests ensure correct behavior for:
    /// - Block-scope variable resolution with lexical scope stack
    /// - GetLocalVariableType correctly finding block-scope locals
    /// - Multiple block scopes with shadowed variable names
    /// </summary>
    public class VariablesTests
    {
        private readonly VariableRegistry _registry;
        private readonly MetadataBuilder _metadataBuilder;
        private readonly BaseClassLibraryReferences _bclReferences;

        public VariablesTests()
        {
            _registry = new VariableRegistry();
            _metadataBuilder = new MetadataBuilder();
            _bclReferences = new BaseClassLibraryReferences(_metadataBuilder);
        }

        /// <summary>
        /// Helper method to register a block-scope uncaptured variable with proper setup.
        /// This simulates what the type generator does for block-scope variables.
        /// </summary>
        private void RegisterBlockScopeVariable(string scopeName, string variableName, Type? clrType, bool isStableType)
        {
            var scopeTypeHandle = CreateTestTypeHandle();
            
            // Mark as uncaptured (no field backing)
            _registry.MarkAsUncaptured(scopeName, variableName);
            
            // Add variable info with type information
            _registry.AddVariable(scopeName, variableName, VariableType.Variable, 
                default, scopeTypeHandle, BindingKind.Const, clrType, isStableType);
        }

        #region Block Scope Variable Resolution Tests

        [Fact]
        public void FindVariable_BlockScopeUncapturedVariable_ShouldResolveWithCorrectClrType()
        {
            // Arrange: Create a function scope with an uncaptured variable in a block scope
            var functionScope = "TestFunction";
            var blockScope = "Block_L10C5";
            var variableName = "blockVar";

            RegisterBlockScopeVariable(blockScope, variableName, typeof(double), isStableType: true);

            var variables = new Variables(_registry, functionScope);

            // Act: Push the block scope and resolve the variable
            variables.PushLexicalScope(blockScope);
            var resolved = variables.FindVariable(variableName);
            variables.PopLexicalScope(blockScope);

            // Assert
            Assert.NotNull(resolved);
            Assert.Equal(typeof(double), resolved.ClrType);
            Assert.True(resolved.IsStableType);
            Assert.True(resolved.LocalSlot >= 0, "Block scope variable should have a local slot");
        }

        [Fact]
        public void FindVariable_BlockScopeVariable_ShouldGetUniqueLocalSlot()
        {
            // Arrange
            var functionScope = "TestFunction";
            var blockScope = "Block_L10C5";

            RegisterBlockScopeVariable(blockScope, "var1", typeof(double), isStableType: true);
            RegisterBlockScopeVariable(blockScope, "var2", typeof(double), isStableType: true);

            var variables = new Variables(_registry, functionScope);

            // Act
            variables.PushLexicalScope(blockScope);
            var var1 = variables.FindVariable("var1");
            var var2 = variables.FindVariable("var2");
            variables.PopLexicalScope(blockScope);

            // Assert: Each variable should have a unique local slot
            Assert.NotNull(var1);
            Assert.NotNull(var2);
            Assert.NotEqual(var1.LocalSlot, var2.LocalSlot);
        }

        [Fact]
        public void FindVariable_NestedBlockScopes_ShouldResolveShadowedVariableCorrectly()
        {
            // Arrange: Two block scopes with same variable name (shadowing)
            var functionScope = "TestFunction";
            var outerBlock = "Block_L10C5";
            var innerBlock = "Block_L15C10";
            var variableName = "x";

            // Outer block has 'x' as string
            RegisterBlockScopeVariable(outerBlock, variableName, typeof(string), isStableType: true);
            
            // Inner block has 'x' as double (shadows outer)
            RegisterBlockScopeVariable(innerBlock, variableName, typeof(double), isStableType: true);

            var variables = new Variables(_registry, functionScope);

            // Act & Assert: Enter outer block
            variables.PushLexicalScope(outerBlock);
            var outerX = variables.FindVariable(variableName);
            Assert.NotNull(outerX);
            Assert.Equal(typeof(string), outerX.ClrType);

            // Enter inner block - should shadow outer x
            variables.PushLexicalScope(innerBlock);
            var innerX = variables.FindVariable(variableName);
            Assert.NotNull(innerX);
            Assert.Equal(typeof(double), innerX.ClrType);
            Assert.NotEqual(outerX.LocalSlot, innerX.LocalSlot);

            // Exit inner block - should see outer x again
            variables.PopLexicalScope(innerBlock);
            var outerXAgain = variables.FindVariable(variableName);
            Assert.NotNull(outerXAgain);
            Assert.Equal(typeof(string), outerXAgain.ClrType);

            variables.PopLexicalScope(outerBlock);
        }

        #endregion

        #region GetLocalVariableType Tests

        [Fact]
        public void GetLocalVariableType_BlockScopeDoubleVariable_ShouldReturnDoubleType()
        {
            // Arrange
            var functionScope = "TestFunction";
            var blockScope = "Block_L10C5";
            var variableName = "numericVar";

            RegisterBlockScopeVariable(blockScope, variableName, typeof(double), isStableType: true);

            var variables = new Variables(_registry, functionScope);

            // Act: Resolve variable to allocate local slot
            variables.PushLexicalScope(blockScope);
            var resolved = variables.FindVariable(variableName);
            variables.PopLexicalScope(blockScope);

            // Get the local variable type for the allocated slot
            var localType = variables.GetLocalVariableType(resolved!.LocalSlot, _bclReferences);

            // Assert: Should return the Double type handle
            Assert.True(localType.HasValue);
            Assert.Equal(_bclReferences.DoubleType, localType.Value);
        }

        [Fact]
        public void GetLocalVariableType_MultipleBlockScopes_AllShouldBeTypedCorrectly()
        {
            // Arrange: Simulate setBitsTrue with two block scopes each having 'bitOffset'
            var functionScope = "setBitsTrue";
            var forLoopBlock = "Block_L66C75";
            var whileLoopBlock = "Block_L83C29";

            // bitOffset in for loop (like line 68)
            RegisterBlockScopeVariable(forLoopBlock, "bitOffset", typeof(double), isStableType: true);
            
            // bitOffset in while loop (like line 84)
            RegisterBlockScopeVariable(whileLoopBlock, "bitOffset", typeof(double), isStableType: true);

            var variables = new Variables(_registry, functionScope);

            // Act: Resolve both block-scope variables
            variables.PushLexicalScope(forLoopBlock);
            var bitOffset1 = variables.FindVariable("bitOffset");
            variables.PopLexicalScope(forLoopBlock);

            variables.PushLexicalScope(whileLoopBlock);
            var bitOffset2 = variables.FindVariable("bitOffset");
            variables.PopLexicalScope(whileLoopBlock);

            // Assert: Both should have float64 type
            Assert.NotNull(bitOffset1);
            Assert.NotNull(bitOffset2);
            Assert.NotEqual(bitOffset1.LocalSlot, bitOffset2.LocalSlot);

            var type1 = variables.GetLocalVariableType(bitOffset1.LocalSlot, _bclReferences);
            var type2 = variables.GetLocalVariableType(bitOffset2.LocalSlot, _bclReferences);

            Assert.True(type1.HasValue);
            Assert.True(type2.HasValue);
            Assert.Equal(_bclReferences.DoubleType, type1.Value);
            Assert.Equal(_bclReferences.DoubleType, type2.Value);
        }

        [Fact]
        public void GetLocalVariableType_NonStableTypeVariable_ShouldReturnNull()
        {
            // Arrange
            var functionScope = "TestFunction";
            var blockScope = "Block_L10C5";
            var variableName = "unstableVar";

            // Variable with ClrType but IsStableType = false
            RegisterBlockScopeVariable(blockScope, variableName, typeof(double), isStableType: false);

            var variables = new Variables(_registry, functionScope);

            // Act
            variables.PushLexicalScope(blockScope);
            var resolved = variables.FindVariable(variableName);
            variables.PopLexicalScope(blockScope);

            var localType = variables.GetLocalVariableType(resolved!.LocalSlot, _bclReferences);

            // Assert: Should return null because IsStableType is false
            Assert.False(localType.HasValue);
        }

        [Fact]
        public void GetLocalVariableType_VariableWithoutClrType_ShouldReturnNull()
        {
            // Arrange
            var functionScope = "TestFunction";
            var blockScope = "Block_L10C5";
            var variableName = "untypedVar";

            // Variable without ClrType
            RegisterBlockScopeVariable(blockScope, variableName, null, isStableType: false);

            var variables = new Variables(_registry, functionScope);

            // Act
            variables.PushLexicalScope(blockScope);
            var resolved = variables.FindVariable(variableName);
            variables.PopLexicalScope(blockScope);

            var localType = variables.GetLocalVariableType(resolved!.LocalSlot, _bclReferences);

            // Assert: Should return null because ClrType is null
            Assert.False(localType.HasValue);
        }

        #endregion

        #region Lexical Scope Stack Tests

        [Fact]
        public void PushPopLexicalScope_ShouldMaintainCorrectOrder()
        {
            // Arrange
            var functionScope = "TestFunction";
            var block1 = "Block_L10C5";
            var block2 = "Block_L20C10";
            var block3 = "Block_L30C15";

            // Each block has a unique variable
            RegisterBlockScopeVariable(block1, "a", typeof(double), isStableType: true);
            RegisterBlockScopeVariable(block2, "b", typeof(double), isStableType: true);
            RegisterBlockScopeVariable(block3, "c", typeof(double), isStableType: true);

            var variables = new Variables(_registry, functionScope);

            // Act & Assert: Push three blocks, then pop in reverse order
            variables.PushLexicalScope(block1);
            Assert.NotNull(variables.FindVariable("a"));

            variables.PushLexicalScope(block2);
            Assert.NotNull(variables.FindVariable("b"));

            variables.PushLexicalScope(block3);
            Assert.NotNull(variables.FindVariable("c"));

            // Pop block3 - should still find a and b
            variables.PopLexicalScope(block3);
            Assert.NotNull(variables.FindVariable("a"));
            Assert.NotNull(variables.FindVariable("b"));

            // Pop block2 - should still find a
            variables.PopLexicalScope(block2);
            Assert.NotNull(variables.FindVariable("a"));

            // Pop block1
            variables.PopLexicalScope(block1);
        }

        [Fact]
        public void GetLeafScopeName_ShouldReturnFunctionScopeName()
        {
            // Arrange
            var functionScope = "TestFunction";
            var variables = new Variables(_registry, functionScope);

            // Act
            var leafScope = variables.GetLeafScopeName();

            // Assert: GetLeafScopeName returns the function scope, not block scopes
            Assert.Equal(functionScope, leafScope);
        }

        #endregion

        #region Local Slot Allocation Tests

        [Fact]
        public void AllocateLocalSlot_SameVariableInDifferentScopes_ShouldGetDifferentSlots()
        {
            // Arrange
            var functionScope = "TestFunction";
            var block1 = "Block_L10C5";
            var block2 = "Block_L20C10";
            var variableName = "i"; // Same name in both blocks

            RegisterBlockScopeVariable(block1, variableName, typeof(double), isStableType: true);
            RegisterBlockScopeVariable(block2, variableName, typeof(double), isStableType: true);

            var variables = new Variables(_registry, functionScope);

            // Act: Resolve 'i' in both block scopes
            variables.PushLexicalScope(block1);
            var i1 = variables.FindVariable(variableName);
            variables.PopLexicalScope(block1);

            variables.PushLexicalScope(block2);
            var i2 = variables.FindVariable(variableName);
            variables.PopLexicalScope(block2);

            // Assert: Each 'i' should have its own local slot
            Assert.NotNull(i1);
            Assert.NotNull(i2);
            Assert.NotEqual(i1.LocalSlot, i2.LocalSlot);
        }

        [Fact]
        public void GetNumberOfLocals_ShouldIncludeBlockScopeLocals()
        {
            // Arrange
            var functionScope = "TestFunction";
            var blockScope = "Block_L10C5";

            RegisterBlockScopeVariable(blockScope, "x", typeof(double), isStableType: true);
            RegisterBlockScopeVariable(blockScope, "y", typeof(double), isStableType: true);

            var variables = new Variables(_registry, functionScope);

            // Initial local count
            var initialCount = variables.GetNumberOfLocals();

            // Act: Resolve block-scope variables to allocate their slots
            variables.PushLexicalScope(blockScope);
            variables.FindVariable("x");
            variables.FindVariable("y");
            variables.PopLexicalScope(blockScope);

            var finalCount = variables.GetNumberOfLocals();

            // Assert: Local count should have increased by 2
            Assert.Equal(initialCount + 2, finalCount);
        }

        #endregion

        #region Integration: Simulating Real-World Patterns

        [Fact]
        public void IntegrationTest_SimulateSetBitsTruePattern()
        {
            // This test simulates the PrimeJavaScript.js setBitsTrue method pattern:
            // - A function with multiple block scopes
            // - Each block has variables initialized from bitwise operations (float64)
            // - Variables like 'bitOffset' appear in multiple blocks

            var functionScope = "setBitsTrue";
            var forLoopBlock = "Block_L66C75";
            var whileLoopBlock = "Block_L83C29";

            // For loop block variables (line 66-75)
            RegisterBlockScopeVariable(forLoopBlock, "wordOffset", typeof(double), isStableType: true);
            RegisterBlockScopeVariable(forLoopBlock, "bitOffset", typeof(double), isStableType: true);
            RegisterBlockScopeVariable(forLoopBlock, "mask", typeof(double), isStableType: true);

            // While loop block variables (line 83-95)
            RegisterBlockScopeVariable(whileLoopBlock, "bitOffset", typeof(double), isStableType: true);
            RegisterBlockScopeVariable(whileLoopBlock, "newwordOffset", typeof(double), isStableType: true);

            var variables = new Variables(_registry, functionScope);

            // Simulate code generation entering for loop block
            variables.PushLexicalScope(forLoopBlock);
            var forWordOffset = variables.FindVariable("wordOffset");
            var forBitOffset = variables.FindVariable("bitOffset");
            var forMask = variables.FindVariable("mask");
            variables.PopLexicalScope(forLoopBlock);

            // Simulate code generation entering while loop block
            variables.PushLexicalScope(whileLoopBlock);
            var whileBitOffset = variables.FindVariable("bitOffset");
            var whileNewWordOffset = variables.FindVariable("newwordOffset");
            variables.PopLexicalScope(whileLoopBlock);

            // Assert: All variables should be resolved with float64 type
            Assert.NotNull(forWordOffset);
            Assert.NotNull(forBitOffset);
            Assert.NotNull(forMask);
            Assert.NotNull(whileBitOffset);
            Assert.NotNull(whileNewWordOffset);

            // All should be stable double types
            Assert.Equal(typeof(double), forWordOffset.ClrType);
            Assert.Equal(typeof(double), forBitOffset.ClrType);
            Assert.Equal(typeof(double), forMask.ClrType);
            Assert.Equal(typeof(double), whileBitOffset.ClrType);
            Assert.Equal(typeof(double), whileNewWordOffset.ClrType);

            // The two bitOffset variables should have different local slots
            Assert.NotEqual(forBitOffset.LocalSlot, whileBitOffset.LocalSlot);

            // Verify GetLocalVariableType returns Double for all
            Assert.Equal(_bclReferences.DoubleType, variables.GetLocalVariableType(forWordOffset.LocalSlot, _bclReferences));
            Assert.Equal(_bclReferences.DoubleType, variables.GetLocalVariableType(forBitOffset.LocalSlot, _bclReferences));
            Assert.Equal(_bclReferences.DoubleType, variables.GetLocalVariableType(forMask.LocalSlot, _bclReferences));
            Assert.Equal(_bclReferences.DoubleType, variables.GetLocalVariableType(whileBitOffset.LocalSlot, _bclReferences));
            Assert.Equal(_bclReferences.DoubleType, variables.GetLocalVariableType(whileNewWordOffset.LocalSlot, _bclReferences));
        }

        #endregion

        #region Helper Methods

        private TypeDefinitionHandle CreateTestTypeHandle()
        {
            return _metadataBuilder.AddTypeDefinition(
                TypeAttributes.Public | TypeAttributes.Class,
                _metadataBuilder.GetOrAddString("TestNamespace"),
                _metadataBuilder.GetOrAddString($"TestType_{Guid.NewGuid()}"),
                default,
                MetadataTokens.FieldDefinitionHandle(1),
                MetadataTokens.MethodDefinitionHandle(1)
            );
        }

        #endregion
    }
}
