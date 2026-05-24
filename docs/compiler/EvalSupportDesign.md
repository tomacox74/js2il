# JavaScript `eval` support design

This document proposes a path to support JavaScript `eval` in JS2IL while preserving ECMAScript semantics and the current ahead-of-time compilation model.

## Status

Proposed design.

JS2IL currently has only deliberately narrow direct-eval handling for a few string-literal test262 lexical-environment ports. General `eval` is still rejected during validation with a compile-time diagnostic. This document describes the target design for full support.

## Goals

- Support direct and indirect `eval` according to ECMA-262.
- Preserve JS2IL's default ahead-of-time compilation model for code that does not use `eval`.
- Keep generated code fast for non-eval paths and isolate eval-related deoptimizations to eval-sensitive scopes.
- Keep the base runtime dependency-light for programs that do not need eval.
- Provide clear host and CLI policy for enabling or disabling dynamic code execution.
- Use test262 eval coverage as the primary correctness target.

## Non-goals

- Making eval fast in the first implementation. Correctness and compatibility come first.
- Supporting dynamic `import()` or module-source parsing through `eval`; ECMAScript `eval` parses script text, not module text.
- Treating eval as a sandbox boundary. Eval executes with the authority of the current realm/host unless the host disables it.
- Replacing the normal AST -> HIR -> LIR -> IL pipeline for statically known source.

## Required semantics

### Direct eval

A call is direct eval only when the callee is the current intrinsic `eval` binding and the syntactic form is a direct call such as:

```js
eval(source)
```

Direct eval must:

- return the argument unchanged when the first argument is not a string,
- parse string input as ECMAScript script source,
- execute in the current realm,
- use the caller's current `this` binding,
- see the caller's active lexical environment chain,
- apply strict/sloppy eval declaration-instantiation rules,
- return the eval script completion value, or `undefined` when there is no value completion,
- preserve abrupt completions by throwing the corresponding JavaScript error.

Sloppy direct eval may introduce or update `var` and function bindings in the caller's variable environment. Strict direct eval, or eval source that contains a strict directive, must keep eval declarations local to the eval execution.

### Indirect eval

Indirect eval includes:

```js
(0, eval)(source)
const e = eval;
e(source);
globalThis.eval(source);
```

Indirect eval must:

- return non-string arguments unchanged,
- execute string source as global script code,
- not capture the caller's lexical environment,
- use the global environment and global `this`,
- introduce sloppy `var` and function declarations as global bindings/properties when permitted.

### Shadowing

If a local binding named `eval` shadows the intrinsic, calls to that binding are ordinary calls. Direct-eval handling must not trigger solely because the callee identifier text is `"eval"`.

## Key design choice: optional dynamic runtime

Full eval requires parsing and executing source strings at runtime. Adding a parser/compiler dependency directly to `JavaScriptRuntime` would conflict with the existing runtime goal that normal compiled output only needs .NET plus `JavaScriptRuntime`.

Use an optional dynamic-code package:

- `JavaScriptRuntime` owns the stable eval facade and policy types.
- A new optional package, tentatively `Js2IL.DynamicRuntime`, owns the runtime parser and dynamic evaluator/compiler.
- Generated assemblies call the stable facade, for example `EvalRuntime.Evaluate(context, source)`.
- When a program contains reachable eval, the CLI includes the optional dynamic runtime next to the generated assembly by default.
- If the optional runtime is absent, the facade throws a targeted `EvalError`/host diagnostic that explains the missing eval support package.

This keeps non-eval programs dependency-light while making eval low-friction for CLI users.

## Compiler design

### 1. Eval classification

Add a compiler analysis pass that classifies eval uses before validation rejects unknown identifiers:

- `NoEval`: no syntactic eval call sites.
- `PotentialIndirectEval`: eval value may be read or called indirectly.
- `DirectEval`: a syntactic direct eval call exists and the `eval` name is not shadowed at that site.
- `EvalShadowed`: the identifier text is `eval`, but normal binding resolution proves it is not the intrinsic.

The pass should annotate call sites with an `EvalCallKind` instead of letting later lowering rediscover eval from raw syntax.

### 2. Eval-sensitive scopes

Any function, script, block, or class-element scope that can be observed by direct eval becomes eval-sensitive.

Eval-sensitive lowering must be conservative:

- materialize all direct-eval-visible bindings in scope objects or runtime environment frames,
- avoid parameter-only storage for parameters visible to eval,
- avoid optimizing unresolved identifiers to hard compile errors when a preceding or reachable sloppy direct eval could introduce them,
- preserve TDZ and const assignment behavior for lexical bindings,
- keep parent scope instances available at runtime even if normal free-variable analysis would otherwise omit them.

This is the main deoptimization boundary. Scopes that cannot be reached by direct eval continue using the existing optimized scope ABI.

### 3. Binding access descriptors

Generate metadata that lets the runtime eval engine read and write existing compiled bindings. Each eval-sensitive scope should expose an `EvalEnvironmentDescriptor` containing entries like:

| Field | Purpose |
| --- | --- |
| `Name` | JavaScript binding name |
| `Kind` | `var`, function, parameter, `let`, `const`, class, catch parameter, or global property |
| `Mutability` | writable, const, initialized/TDZ-sensitive |
| `Storage` | scope field, parameter field, global property, or dynamic overlay |
| `DeclaringScopeId` | stable compiler identity for diagnostics and closure capture |

For normal compiled bindings, the descriptor maps a name to generated scope fields. For eval-created sloppy `var` bindings that were not known at compile time, the descriptor points to a dynamic overlay dictionary attached to the variable environment.

### 4. Dynamic overlays for eval-created bindings

Static scope classes cannot grow fields at runtime. Sloppy direct eval can still create bindings that later compiled code must see:

```js
eval("var x = 1");
console.log(x);
```

Eval-sensitive variable environments therefore need a dynamic overlay:

- known bindings keep using generated fields,
- eval-created `var`/function bindings live in a per-environment dictionary,
- unresolved identifier loads/stores in eval-sensitive code lower to a runtime lookup that checks generated bindings, then the dynamic overlay, then the outer environment/global rules,
- strict unresolved assignment still throws instead of creating a binding.

The overlay should be absent for non-eval-sensitive scopes.

### 5. HIR/LIR additions

Introduce explicit IR nodes rather than special-casing eval as a normal call:

- `HIREvalExpression`
  - source expression,
  - direct vs indirect kind,
  - source location,
  - strictness context,
  - current lexical/variable environment handles for direct eval.
- `HIRCreateEvalContext`
  - captures current realm/global object,
  - current `this`,
  - CommonJS module context when applicable,
  - eval environment descriptors.
- `HIRDynamicBindingRead` / `HIRDynamicBindingWrite`
  - used only in eval-sensitive scopes where an identifier may be introduced dynamically.

Lowering emits calls through stable runtime facade methods. The dynamic runtime decides whether to interpret or dynamically compile the eval source.

### 6. Validation changes

Validation should no longer reject every identifier named `eval`. Instead:

- reject eval only when the project/host policy disables dynamic code,
- allow direct and indirect eval when the dynamic runtime is available or will be packaged,
- keep targeted diagnostics for unsupported eval subfeatures during staged rollout,
- include the source location and whether the call is direct or indirect in diagnostics.

For staged rollout, unsupported eval cases should fail with explicit diagnostics rather than silently falling back to wrong semantics.

## Runtime design

### 1. Eval facade

Add a facade in `JavaScriptRuntime`, for example:

```csharp
public static class EvalRuntime
{
    public static object? Evaluate(EvalContext context, object? source);
}
```

The facade handles the required non-string shortcut:

```js
eval(123) === 123
```

For string input, it delegates to an `IJavaScriptEvalService` registered by the optional dynamic runtime.

### 2. Eval context

`EvalContext` should contain:

- realm/global object,
- current `this`,
- strictness of the caller,
- direct vs indirect mode,
- source location and filename for diagnostics,
- current lexical environment chain for direct eval,
- current variable environment for sloppy direct eval declaration binding,
- CommonJS module metadata (`require`, `module`, `exports`, `__filename`, `__dirname`) when eval runs inside a CommonJS module,
- host policy.

### 3. Environment records

The dynamic runtime should use a small runtime environment-record model aligned with ECMA-262:

- declarative environment records for eval-local lexical declarations,
- object/global environment records for global eval,
- bridge records over generated scope fields,
- dynamic overlay records for eval-created sloppy vars/functions.

Bridge records should use generated descriptors and strongly typed access helpers instead of reflection-heavy per-access lookup in hot paths. Reflection is acceptable for the first implementation if isolated behind descriptor caches.

### 4. Execution strategy

Start with an interpreter in `Js2IL.DynamicRuntime`.

Reasons:

- eval is rare relative to normal execution,
- interpreter semantics are easier to make correct for declaration instantiation, TDZ, completion values, and dynamic binding creation,
- no runtime IL metadata ordering constraints,
- functions created inside eval can capture interpreter environment records without forcing new generated scope classes.

After correctness is established, add an optional dynamic-IL tier:

- cache parsed/compiled eval source by source text, strictness, and visible environment shape,
- compile eval code that does not introduce hard dynamic-binding hazards,
- fall back to the interpreter for complex cases.

### 5. Functions created by eval

Functions, arrows, classes, and generators created inside eval must close over the eval environment. The first implementation can represent them as runtime function objects backed by the dynamic runtime interpreter.

Later, dynamic IL compilation can emit functions with an environment-record ABI instead of scope-as-class fields. This ABI should be separate from the normal ahead-of-time `CallableScopeAbiKind` so eval does not destabilize existing generated callable metadata.

## Host policy

Eval support should be controlled by host policy:

| Policy | Behavior |
| --- | --- |
| `Enabled` | eval executes when the dynamic runtime is available |
| `Disabled` | any string eval throws a targeted host `EvalError` or policy exception |
| `LiteralOnly` | optional transitional mode that allows compile-time-known eval literals but rejects runtime strings |

Recommended defaults:

- CLI: `Enabled` when eval is used, packaging `Js2IL.DynamicRuntime` automatically.
- Hosting API: `Enabled` by default for Node/ECMA compatibility, with an explicit option to disable for sandboxed hosts.
- Test262 runner: `Enabled`, except for cases requiring features that remain unsupported independently of eval.

## Rollout plan

### Phase 0: keep current narrow support explicit

- Document the current string-literal-only support as temporary.
- Keep existing eval-related skips linked to the eval tracking issue.

### Phase 1: indirect eval

- Add the `eval` global function object.
- Route indirect eval through `EvalRuntime.Evaluate` with the global environment.
- Support non-string argument passthrough.
- Support global script execution with no local lexical capture.

This phase avoids the hardest direct-eval scope mutation issues.

### Phase 2: strict direct eval

- Add `HIREvalExpression` and direct eval context capture.
- Support direct eval where caller strictness or eval source strictness prevents var/function leakage into the caller.
- Support eval-local lexical declarations and closures using dynamic environment records.

### Phase 3: sloppy direct eval with known bindings

- Bridge generated scope fields into runtime environment records.
- Allow direct eval to read/write existing parameters, vars, lets, consts, and globals correctly.
- Preserve TDZ and strict assignment behavior.

### Phase 4: sloppy direct eval with new bindings

- Add dynamic overlays for eval-created vars/functions.
- Lower unresolved identifier access in eval-sensitive scopes to dynamic binding lookup.
- Support post-eval access to newly introduced sloppy var/function bindings.

### Phase 5: optimization

- Cache parsed eval programs.
- Add dynamic IL compilation for safe cases.
- Re-enable local optimizations in eval-sensitive scopes when analysis proves they cannot be affected by eval-created bindings.

## Test strategy

Use test262 as the primary acceptance source. Focus first on:

- non-string eval argument passthrough,
- direct vs indirect eval `this` and environment behavior,
- shadowed eval behaving as an ordinary call,
- strict direct eval not leaking declarations,
- sloppy direct eval leaking `var` and function declarations,
- lexical declarations and TDZ inside eval,
- closures created inside eval,
- eval completion values,
- syntax errors and runtime abrupt completions,
- global eval interactions with CommonJS module globals.

Project-local tests should cover packaging and hosting behavior that test262 does not cover:

- CLI output includes the optional dynamic runtime when eval is reachable,
- hosting API policy can disable eval with a clear diagnostic,
- missing optional dynamic runtime produces an actionable error,
- source locations for eval parse/runtime errors include the eval call site and eval source name.

## Open questions

- Should `Js2IL.DynamicRuntime` be copied only when static analysis sees eval, or always with CLI output for simplicity?
- Should hosting default to eval enabled for compatibility, or disabled for safer embedding?
- How should source maps/PDB sequence points represent dynamically evaluated code?
- How much of the existing AST -> HIR lowering can the dynamic interpreter share without pulling compiler dependencies into the runtime package?
- Should eval-created functions eventually be dynamically compiled to IL, or is interpreter execution acceptable indefinitely?

## Alternatives considered

### Always reject eval

This keeps the compiler simple but blocks significant test262 and Node compatibility. It is not aligned with the long-term compatibility goals.

### Inline only string-literal direct eval

This extends the current narrow approach and is useful for a few tests, but it is not real eval support. Runtime-generated strings, indirect eval, sloppy declaration leakage, and shadowing semantics remain incorrect.

### Add Acornima directly to JavaScriptRuntime

This is the simplest implementation shape, but it adds parser dependencies to every compiled program even when eval is unused. The optional dynamic runtime keeps the default deployment story small.

### Recompile the whole enclosing function at runtime

This could preserve optimized field access after eval introduces bindings, but it is complex, hard to make deterministic, and risky for stack/closure state. Dynamic overlays plus eval-sensitive deoptimization are simpler and more correct for the first implementation.
