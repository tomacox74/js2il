using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using Js2IL.Services.VariableBindings;
using Xunit;

namespace Js2IL.Tests
{
    public class VariableRegistryTests
    {
        private readonly VariableRegistry _registry;
        private readonly MetadataBuilder _metadataBuilder;

        public VariableRegistryTests()
        {
            _registry = new VariableRegistry();
            _metadataBuilder = new MetadataBuilder();
        }

        [Fact]
        public void AddVariable_ShouldAddVariableToRegistry()
        {
            // Arrange
            var scopeName = "TestScope";
            var variableName = "testVar";
            var variableType = VariableType.Variable;
            var fieldHandle = CreateTestFieldHandle();
            var scopeTypeHandle = CreateTestTypeHandle();

            // Act
            _registry.AddVariable(scopeName, variableName, variableType, fieldHandle, scopeTypeHandle);

            // Assert
            var variables = _registry.GetVariablesForScope(scopeName);
            Assert.Single(variables);
            
            var variable = variables.First();
            Assert.Equal(variableName, variable.Name);
            Assert.Equal(scopeName, variable.ScopeName);
            Assert.Equal(variableType, variable.VariableType);
            Assert.Equal(fieldHandle, variable.FieldHandle);
            Assert.Equal(scopeTypeHandle, variable.ScopeTypeHandle);
        }

        [Fact]
        public void AddVariable_MultipleScopesAndVariables_ShouldOrganizeCorrectly()
        {
            // Arrange
            var scope1 = "Scope1";
            var scope2 = "Scope2";
            var var1 = "variable1";
            var var2 = "variable2";
            var var3 = "variable3";

            var field1 = CreateTestFieldHandle();
            var field2 = CreateTestFieldHandle();
            var field3 = CreateTestFieldHandle();
            var type1 = CreateTestTypeHandle();
            var type2 = CreateTestTypeHandle();

            // Act
            _registry.AddVariable(scope1, var1, VariableType.Variable, field1, type1);
            _registry.AddVariable(scope1, var2, VariableType.Function, field2, type1);
            _registry.AddVariable(scope2, var3, VariableType.Parameter, field3, type2);

            // Assert
            var scope1Variables = _registry.GetVariablesForScope(scope1).ToList();
            var scope2Variables = _registry.GetVariablesForScope(scope2).ToList();

            Assert.Equal(2, scope1Variables.Count);
            Assert.Single(scope2Variables);

            Assert.Contains(scope1Variables, v => v.Name == var1 && v.VariableType == VariableType.Variable);
            Assert.Contains(scope1Variables, v => v.Name == var2 && v.VariableType == VariableType.Function);
            Assert.Contains(scope2Variables, v => v.Name == var3 && v.VariableType == VariableType.Parameter);
        }

        [Fact]
        public void GetVariablesForScope_NonExistentScope_ShouldReturnEmptyCollection()
        {
            // Act
            var variables = _registry.GetVariablesForScope("NonExistentScope");

            // Assert
            Assert.Empty(variables);
        }

        [Fact]
        public void GetFieldHandle_ExistingVariable_ShouldReturnCorrectHandle()
        {
            // Arrange
            var scopeName = "TestScope";
            var variableName = "testVar";
            var expectedFieldHandle = CreateTestFieldHandle();
            var scopeTypeHandle = CreateTestTypeHandle();

            _registry.AddVariable(scopeName, variableName, VariableType.Variable, expectedFieldHandle, scopeTypeHandle);

            // Act
            var actualFieldHandle = _registry.GetFieldHandle(scopeName, variableName);

            // Assert
            Assert.Equal(expectedFieldHandle, actualFieldHandle);
        }

        [Fact]
        public void GetFieldHandle_NonExistentScope_ShouldThrowKeyNotFoundException()
        {
            // Act & Assert
            Assert.Throws<KeyNotFoundException>(() => _registry.GetFieldHandle("NonExistentScope", "someVar"));
        }

        [Fact]
        public void GetFieldHandle_NonExistentVariable_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            var scopeName = "TestScope";
            var existingVar = "existingVar";
            var nonExistentVar = "nonExistentVar";
            var fieldHandle = CreateTestFieldHandle();
            var scopeTypeHandle = CreateTestTypeHandle();

            _registry.AddVariable(scopeName, existingVar, VariableType.Variable, fieldHandle, scopeTypeHandle);

            // Act & Assert
            Assert.Throws<KeyNotFoundException>(() => _registry.GetFieldHandle(scopeName, nonExistentVar));
        }

        [Fact]
        public void GetScopeTypeHandle_ExistingScope_ShouldReturnCorrectHandle()
        {
            // Arrange
            var scopeName = "TestScope";
            var variableName = "testVar";
            var fieldHandle = CreateTestFieldHandle();
            var expectedScopeTypeHandle = CreateTestTypeHandle();

            _registry.AddVariable(scopeName, variableName, VariableType.Variable, fieldHandle, expectedScopeTypeHandle);

            // Act
            var actualScopeTypeHandle = _registry.GetScopeTypeHandle(scopeName);

            // Assert
            Assert.Equal(expectedScopeTypeHandle, actualScopeTypeHandle);
        }

        [Fact]
        public void GetScopeTypeHandle_NonExistentScope_ShouldThrowKeyNotFoundException()
        {
            // Act & Assert
            Assert.Throws<KeyNotFoundException>(() => _registry.GetScopeTypeHandle("NonExistentScope"));
        }

        [Fact]
        public void GetAllScopeNames_EmptyRegistry_ShouldReturnEmptyCollection()
        {
            // Act
            var scopeNames = _registry.GetAllScopeNames();

            // Assert
            Assert.Empty(scopeNames);
        }

        [Fact]
        public void GetAllScopeNames_WithScopes_ShouldReturnAllScopeNames()
        {
            // Arrange
            var scope1 = "Scope1";
            var scope2 = "Scope2";
            var scope3 = "Scope3";

            var fieldHandle = CreateTestFieldHandle();
            var typeHandle = CreateTestTypeHandle();

            _registry.AddVariable(scope1, "var1", VariableType.Variable, fieldHandle, typeHandle);
            _registry.AddVariable(scope2, "var2", VariableType.Function, fieldHandle, typeHandle);
            _registry.AddVariable(scope3, "var3", VariableType.Parameter, fieldHandle, typeHandle);

            // Act
            var scopeNames = _registry.GetAllScopeNames().ToList();

            // Assert
            Assert.Equal(3, scopeNames.Count);
            Assert.Contains(scope1, scopeNames);
            Assert.Contains(scope2, scopeNames);
            Assert.Contains(scope3, scopeNames);
        }

        [Fact]
        public void FindVariable_ExistingVariable_ShouldReturnCorrectVariableInfo()
        {
            // Arrange
            var scopeName = "TestScope";
            var variableName = "testVar";
            var variableType = VariableType.Function;
            var fieldHandle = CreateTestFieldHandle();
            var scopeTypeHandle = CreateTestTypeHandle();

            _registry.AddVariable(scopeName, variableName, variableType, fieldHandle, scopeTypeHandle);

            // Act
            var foundVariable = _registry.FindVariable(variableName);

            // Assert
            Assert.NotNull(foundVariable);
            Assert.Equal(variableName, foundVariable.Name);
            Assert.Equal(scopeName, foundVariable.ScopeName);
            Assert.Equal(variableType, foundVariable.VariableType);
            Assert.Equal(fieldHandle, foundVariable.FieldHandle);
            Assert.Equal(scopeTypeHandle, foundVariable.ScopeTypeHandle);
        }

        [Fact]
        public void FindVariable_NonExistentVariable_ShouldReturnNull()
        {
            // Arrange
            _registry.AddVariable("TestScope", "existingVar", VariableType.Variable, CreateTestFieldHandle(), CreateTestTypeHandle());

            // Act
            var foundVariable = _registry.FindVariable("nonExistentVar");

            // Assert
            Assert.Null(foundVariable);
        }

        [Fact]
        public void FindVariable_VariableInMultipleScopes_ShouldReturnFirstMatch()
        {
            // Arrange
            var variableName = "sharedVar";
            var scope1 = "Scope1";
            var scope2 = "Scope2";

            var field1 = CreateTestFieldHandle();
            var field2 = CreateTestFieldHandle();
            var type1 = CreateTestTypeHandle();
            var type2 = CreateTestTypeHandle();

            _registry.AddVariable(scope1, variableName, VariableType.Variable, field1, type1);
            _registry.AddVariable(scope2, variableName, VariableType.Function, field2, type2);

            // Act
            var foundVariable = _registry.FindVariable(variableName);

            // Assert
            Assert.NotNull(foundVariable);
            Assert.Equal(variableName, foundVariable.Name);
            // Should return the first one found (implementation dependent on dictionary ordering)
            Assert.True(foundVariable.ScopeName == scope1 || foundVariable.ScopeName == scope2);
        }

        [Fact]
        public void VariableType_Enum_ShouldHaveExpectedValues()
        {
            // Assert
            Assert.True(Enum.IsDefined(typeof(VariableType), VariableType.Variable));
            Assert.True(Enum.IsDefined(typeof(VariableType), VariableType.Function));
            Assert.True(Enum.IsDefined(typeof(VariableType), VariableType.Parameter));
        }

        [Fact]
        public void VariableInfo_Properties_ShouldBeSettable()
        {
            // Arrange
            var variableInfo = new VariableInfo();
            var name = "testVar";
            var scopeName = "TestScope";
            var variableType = VariableType.Function;
            var fieldHandle = CreateTestFieldHandle();
            var scopeTypeHandle = CreateTestTypeHandle();

            // Act
            variableInfo.Name = name;
            variableInfo.ScopeName = scopeName;
            variableInfo.VariableType = variableType;
            variableInfo.FieldHandle = fieldHandle;
            variableInfo.ScopeTypeHandle = scopeTypeHandle;

            // Assert
            Assert.Equal(name, variableInfo.Name);
            Assert.Equal(scopeName, variableInfo.ScopeName);
            Assert.Equal(variableType, variableInfo.VariableType);
            Assert.Equal(fieldHandle, variableInfo.FieldHandle);
            Assert.Equal(scopeTypeHandle, variableInfo.ScopeTypeHandle);
        }

        [Fact]
        public void AddVariable_SameVariableNameDifferentScopes_ShouldAllowBothVariables()
        {
            // Arrange
            var variableName = "sameVarName";
            var scope1 = "Scope1";
            var scope2 = "Scope2";
            var field1 = CreateTestFieldHandle();
            var field2 = CreateTestFieldHandle();
            var type1 = CreateTestTypeHandle();
            var type2 = CreateTestTypeHandle();

            // Act
            _registry.AddVariable(scope1, variableName, VariableType.Variable, field1, type1);
            _registry.AddVariable(scope2, variableName, VariableType.Function, field2, type2);

            // Assert
            var scope1Variables = _registry.GetVariablesForScope(scope1);
            var scope2Variables = _registry.GetVariablesForScope(scope2);

            Assert.Single(scope1Variables);
            Assert.Single(scope2Variables);

            var var1 = scope1Variables.First();
            var var2 = scope2Variables.First();

            Assert.Equal(variableName, var1.Name);
            Assert.Equal(scope1, var1.ScopeName);
            Assert.Equal(VariableType.Variable, var1.VariableType);

            Assert.Equal(variableName, var2.Name);
            Assert.Equal(scope2, var2.ScopeName);
            Assert.Equal(VariableType.Function, var2.VariableType);
        }

        #region Helper Methods

        private FieldDefinitionHandle CreateTestFieldHandle()
        {
            // Create a minimal field definition for testing
            var fieldSignature = new BlobBuilder();
            new BlobEncoder(fieldSignature)
                .Field()
                .Type()
                .Object();

            return _metadataBuilder.AddFieldDefinition(
                FieldAttributes.Public,
                _metadataBuilder.GetOrAddString($"TestField_{Guid.NewGuid()}"),
                _metadataBuilder.GetOrAddBlob(fieldSignature)
            );
        }

        private TypeDefinitionHandle CreateTestTypeHandle()
        {
            // Create a minimal type definition for testing
            return _metadataBuilder.AddTypeDefinition(
                TypeAttributes.Public | TypeAttributes.Class,
                _metadataBuilder.GetOrAddString("TestNamespace"),
                _metadataBuilder.GetOrAddString($"TestType_{Guid.NewGuid()}"),
                default, // no base type for test
                MetadataTokens.FieldDefinitionHandle(1), // first field
                MetadataTokens.MethodDefinitionHandle(1) // first method
            );
        }

        #endregion
    }
}
