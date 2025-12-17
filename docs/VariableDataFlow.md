# Variable Data Flow: From Discovery to IL Emission

This document describes the complete data flow for a JavaScript variable from its discovery during parsing through to IL emission in JS2IL.

## Overview

A JavaScript variable goes through 5 major phases before becoming IL bytecode:

```
┌─────────────┐    ┌───────────────┐    ┌─────────────────┐    ┌────────────────┐    ┌─────────────┐
│   Parse     │ -> │ Symbol Table  │ -> │ Type Generation │ -> │ IL Generation  │ -> │  Emitted    │
│  (Acornima) │    │   Building    │    │   (TypeGenerator)│   │  (Variables)   │    │    IL       │
└─────────────┘    └───────────────┘    └─────────────────┘    └────────────────┘    └─────────────┘
```

## Phase 1: Parsing (AST Creation)

**File**: External library (Acornima)

The JavaScript source is parsed into an Abstract Syntax Tree (AST). Variable declarations appear as specific node types:

| JavaScript | AST Node Type |
|------------|---------------|
| `var x = 1` | `VariableDeclaration` with `VariableDeclarator` |
| `let y = 2` | `VariableDeclaration` with `VariableDeclarator` |
| `const z = 3` | `VariableDeclaration` with `VariableDeclarator` |
| `function foo(a)` | `FunctionDeclaration` (params as `Identifier` nodes) |

## Phase 2: Symbol Table Building

**File**: [SymbolTableBuilder.cs](../Js2IL/SymbolTable/SymbolTableBuilder.cs)

### 2.1 Scope Tree Creation

The `SymbolTableBuilder` walks the AST and creates a tree of `Scope` objects. Each JavaScript scope becomes a `Scope` instance:

- **Global scope** → `ScopeKind.Global`
- **Function** → `ScopeKind.Function`
- **Block (`{...}`)** → `ScopeKind.Block`
- **Class** → `ScopeKind.Class`

### 2.2 Binding Discovery

When a `VariableDeclaration` node is encountered, a `BindingInfo` is created:

```csharp
// From SymbolTableBuilder.cs - BuildScopeRecursive()
case VariableDeclaration varDecl:
    var kind = varDecl.Kind switch {
        VariableDeclarationKind.Var => BindingKind.Var,
        VariableDeclarationKind.Let => BindingKind.Let,
        VariableDeclarationKind.Const => BindingKind.Const,
        _ => BindingKind.Var
    };
    var binding = new BindingInfo(id.Name, kind, decl);
    targetScope.Bindings[id.Name] = binding;
```

**Key Data Structures:**

```csharp
// BindingInfo (BindingInfo.cs) - stores variable metadata
public class BindingInfo {
    public string Name { get; }           // Variable name
    public BindingKind Kind { get; }      // Var, Let, Const, Function
    public Node DeclarationNode { get; }  // AST node for error reporting
    public Type? ClrType { get; set; }    // Known CLR type (e.g., from require())
    public bool IsCaptured { get; set; }  // Referenced by child scope?
    public bool IsStableType { get; set; } // Type known to never change?
}

// Scope (Scope.cs) - represents a JavaScript scope
public class Scope {
    public string Name { get; }
    public Scope? Parent { get; }
    public Dictionary<string, BindingInfo> Bindings { get; }
    public HashSet<string> Parameters { get; }           // Function parameter names
    public HashSet<string> DestructuredParameters { get; }
    public bool ReferencesParentScopeVariables { get; set; }
}
```

### 2.3 Var Hoisting

JavaScript `var` declarations are hoisted to the enclosing function/global scope:

```csharp
// var in a block is hoisted to the enclosing function
if (kind == BindingKind.Var && currentScope.Kind == ScopeKind.Block) {
    var ancestor = currentScope.Parent;
    while (ancestor != null && ancestor.Kind != ScopeKind.Function && ancestor.Kind != ScopeKind.Global)
        ancestor = ancestor.Parent;
    if (ancestor != null)
        targetScope = ancestor;
}
```

### 2.4 Free Variable Analysis

After building the scope tree, `AnalyzeFreeVariables()` determines which scopes reference parent scope variables:

```csharp
private void AnalyzeFreeVariables(Scope scope) {
    // Bottom-up: process children first
    foreach (var child in scope.Children)
        AnalyzeFreeVariables(child);
    
    // Mark scope if it references non-local variables
    scope.ReferencesParentScopeVariables = CheckBodyReferencesParentVariables(...);
}
```

### 2.5 Captured Variable Marking

`MarkCapturedVariables()` marks bindings that are referenced by child scopes:

```csharp
// If a child scope references a variable, mark it as captured
// This affects whether a field vs local variable is generated
binding.IsCaptured = true;
```

## Phase 3: Type Generation

**File**: [TypeGenerator.cs](../Js2IL/Services/TypeGenerator.cs)

### 3.1 Scope-as-Class Pattern

Each `Scope` becomes a .NET class. Variables become instance fields on that class:

```csharp
// JavaScript:
function outer() {
    let x = 10;
    function inner() { return x; }
}

// Generated .NET (conceptual):
class outer {
    public object x;  // Field for captured variable
}
```

### 3.2 Field Creation

`CreateTypeFields()` decides which variables need backing fields:

```csharp
private void CreateTypeFields(Scope scope, TypeBuilder typeBuilder) {
    foreach (var binding in scope.Bindings.Values) {
        bool isParameter = scope.Parameters.Contains(binding.Name);
        bool isCaptured = binding.IsCaptured;
        
        // Skip field for uncaptured non-parameter variables (use local instead)
        if (!isCaptured && !isParameter && binding.Kind != BindingKind.Function)
            continue;  // Will use local variable in IL
        
        // Create field: all fields are typed as System.Object
        var fieldHandle = typeBuilder.AddFieldDefinition(
            FieldAttributes.Public,
            binding.Name,
            objectTypeSignature);
    }
}
```

### 3.3 Variable Registry Population

`PopulateVariableRegistry()` creates entries for IL generation:

```csharp
// For captured variables: record field handle
_variableRegistry.AddVariable(
    scopeName,           // "outer" or "Point/constructor"
    variableName,        // "x"
    variableType,        // Parameter, Variable, or Function
    fieldHandle,         // FieldDefinitionHandle
    scopeTypeHandle,     // TypeDefinitionHandle for scope class
    bindingKind,         // Const, Let, Var, Function
    clrType,             // typeof(double), etc. if known
    isStableType);       // true if type won't change

// For uncaptured variables: mark for local allocation
_variableRegistry.MarkAsUncaptured(scopeName, variableName);
```

**Output Data Structure:**

```csharp
// VariableInfo (VariableRegistry.cs)
public class VariableInfo {
    public string Name { get; set; }
    public string ScopeName { get; set; }
    public VariableType VariableType { get; set; }
    public FieldDefinitionHandle FieldHandle { get; set; }
    public TypeDefinitionHandle ScopeTypeHandle { get; set; }
    public BindingKind BindingKind { get; set; }
    public Type? ClrType { get; set; }
    public bool IsStableType { get; set; }
}
```

## Phase 4: IL Generation Setup

**File**: [Variable.cs](../Js2IL/Services/VariableBindings/Variable.cs) (the `Variables` class)

### 4.1 Variables Class Construction

When generating IL for a method, a `Variables` instance is created to resolve variable names:

```csharp
// For global scope:
var variables = new Variables(variableRegistry, scopeName);

// For nested function:
var variables = new Variables(parentVariables, scopeName, parameterNames, isNestedFunction);
```

### 4.2 Variable Resolution

`FindVariable()` maps a variable name to its storage location:

```csharp
public Variable? FindVariable(string name) {
    // 1. Check lexical (block) scope stack for shadowing
    foreach (var scopeName in _lexicalScopeStack) {
        if (TryResolveFieldBackedVariable(scopeName, name, out var v))
            return v;
    }
    
    // 2. Check cached resolutions
    if (_variables.TryGetValue(name, out var cached)) return cached;
    
    // 3. Check if it's a parameter (use ldarg)
    if (_parameterIndices.TryGetValue(name, out var pindex))
        return new ParameterVariable { ... };
    
    // 4. Check if uncaptured (use local variable)
    if (_registry.IsUncaptured(_scopeName, name)) {
        var localSlot = AllocateLocalSlot(_scopeName, name);
        return new LocalVariable { LocalSlot = localSlot, ... };
    }
    
    // 5. Field-backed variable in current or parent scope
    return new ScopeVariable { FieldHandle = ..., ParentScopeIndex = ... };
}
```

**Variable Subtypes:**

| Variable Type | Storage | IL Load Pattern |
|--------------|---------|-----------------|
| `LocalVariable` | Local slot | `ldloc.N` |
| `ParameterVariable` | Method argument | `ldarg.N` |
| `ScopeVariable` (current) | Scope instance field | `ldloc.0; ldfld` |
| `ScopeVariable` (parent) | Parent scope field | `ldarg.0; ldelem_ref; castclass; ldfld` |

## Phase 5: IL Emission

**File**: [ILEmitHelpers.cs](../Js2IL/Services/ILGenerators/ILEmitHelpers.cs)

### 5.1 Loading Variables

`EmitLoadVariable()` generates the appropriate IL based on variable type:

```csharp
public static void EmitLoadVariable(
    this InstructionEncoder il,
    Variable variable,
    Variables variables,
    BaseClassLibraryReferences bclReferences,
    bool unbox = false) 
{
    // Local variable (uncaptured)
    if (variable.LocalSlot >= 0) {
        il.LoadLocal(variable.LocalSlot);  // ldloc.N
        return;
    }
    
    // Parent scope variable (captured from ancestor)
    if (variable is ScopeVariable sv) {
        il.LoadArgument(0);                       // ldarg.0 (scopes array)
        il.LoadConstantI4(sv.ParentScopeIndex);   // ldc.i4 N
        il.OpCode(ILOpCode.Ldelem_ref);           // ldelem.ref
        il.OpCode(ILOpCode.Castclass);            // castclass ScopeType
        il.Token(scopeTypeHandle);
        il.OpCode(ILOpCode.Ldfld);                // ldfld
        il.Token(sv.FieldHandle);
        return;
    }
    
    // Parameter
    if (variable.IsParameter) {
        il.LoadArgument(variable.ParameterIndex); // ldarg.N
        return;
    }
    
    // Current scope field
    il.LoadLocal(scopeLocalIndex);    // ldloc.0 (scope instance)
    il.OpCode(ILOpCode.Ldfld);        // ldfld
    il.Token(variable.FieldHandle);
}
```

### 5.2 Storing Variables

`EmitStoreVariable()` generates IL to store a value:

```csharp
// For local: stloc.N
// For field: ldloc.0; [value on stack]; stfld fieldHandle
```

## Complete Example

### JavaScript Source
```javascript
function counter() {
    let count = 0;
    return function() {
        return ++count;
    };
}
```

### Phase 2: Symbol Table
```
Scope: GlobalScope
├── Binding: counter (Function)
└── Child: Scope "counter"
    ├── Binding: count (Let, IsCaptured=true)
    └── Child: Scope "ArrowFunction_L3C11"
        └── References count (free variable)
```

### Phase 3: Type Generation
```csharp
// Generated class
class counter {
    public object count;  // Field because IsCaptured=true
}
```

### Phase 4: Variable Resolution
```csharp
// In ArrowFunction scope:
variables.FindVariable("count")
// Returns: ScopeVariable { 
//     Name = "count",
//     ScopeName = "counter",
//     ParentScopeIndex = 0,
//     FieldHandle = <handle to count field>
// }
```

### Phase 5: IL Output
```
// Loading 'count' in inner function:
ldarg.0          // Load scopes[] array
ldc.i4.0         // Index 0 = parent scope
ldelem.ref       // Load scope object
castclass counter // Cast to concrete type
ldfld object counter::count // Load the field
```

## Optimization: Uncaptured Variables

Variables that aren't referenced by child scopes skip field creation:

```javascript
function example() {
    let local = 42;  // Not captured - uses local variable
    return local;    // Loads via ldloc.N (faster)
}
```

This optimization is tracked via:
1. `BindingInfo.IsCaptured = false` (Phase 2)
2. `VariableRegistry.MarkAsUncaptured()` (Phase 3)
3. `Variable.LocalSlot >= 0` (Phase 4)
4. `ldloc.N` emission (Phase 5)

## Related Files

- [SymbolTableBuilder.cs](../Js2IL/SymbolTable/SymbolTableBuilder.cs) - Phase 2
- [Scope.cs](../Js2IL/SymbolTable/Scope.cs) - Scope data structure
- [BindingInfo.cs](../Js2IL/SymbolTable/BindingInfo.cs) - Variable metadata
- [TypeGenerator.cs](../Js2IL/Services/TypeGenerator.cs) - Phase 3
- [VariableRegistry.cs](../Js2IL/Services/VariableBindings/VariableRegistry.cs) - Registry
- [Variable.cs](../Js2IL/Services/VariableBindings/Variable.cs) - Phase 4 (Variables class)
- [ILEmitHelpers.cs](../Js2IL/Services/ILGenerators/ILEmitHelpers.cs) - Phase 5
