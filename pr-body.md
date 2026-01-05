## Summary

Expand the AST→HIR→LIR→IL pipeline to support object/array literals and related runtime semantics, while preventing Stackify from duplicating call side effects.

## Changes

### New Features
- **Object literals (IR pipeline)**: Lower `{ ... }` to runtime `ExpandoObject` initialization.
- **Array literals (IR pipeline)**: Lower `[ ... ]` including spread (`[...arr]`) to `JavaScriptRuntime.Array` construction.
- **Computed/index access**: Lower `obj[index]` via runtime `Object.GetItem`.
- **`length` property**: Lower `obj.length` via runtime `Object.GetLength`.
- **General call lowering**:
  - Typed array instance calls via `LIRCallInstanceMethod` (e.g., `arr.join()`, `arr.pop()`, `arr.slice()`), including `slice()` typed as `JavaScriptRuntime.Array` for chaining.
  - Intrinsic static calls via `LIRCallIntrinsicStatic` (e.g., `Math.*`, `Array.isArray`).
- **Correct JS truthiness**: Boxed/object conditions branch through `Operators.IsTruthy(...)`.
- **Stackify correctness**: Never inline/stackify call-like instructions to avoid duplicate invocation in emitted IL.

### Files Added
- Js2IL/IR/HIR/HIRArrayExpression.cs
- Js2IL/IR/HIR/HIRSpreadElement.cs
- Js2IL/IR/HIR/HIRObjectExpression.cs
- Js2IL/IR/HIR/HIRIndexAccessExpression.cs
- Js2IL/IR/LIR/LIRArrayInstructions.cs

### Files Modified
- Js2IL/IR/HIR/HIRBuilder.cs
- Js2IL/IR/LIR/HIRToLIRLower.cs
- Js2IL/IR/LIR/LIRInstructions.cs
- Js2IL/IL/LIRToILCompiler.cs
- Js2IL/IL/TempLocalAllocator.cs
- Js2IL/IL/Stackify.cs
- Js2IL.Tests/Array/GeneratorTests.cs (enforce IR pipeline where required)
- Js2IL.Tests/Literals/GeneratorTests.cs (enforce IR pipeline where required)
- Updated generator snapshots across Array/Literals/Math/Promise/etc.

## Testing
- Focused `dotnet test` run planned after commit (Array Join/Pop/Slice + Literals).