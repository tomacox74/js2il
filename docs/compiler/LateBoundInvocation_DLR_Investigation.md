# Late-Bound Invocation and DLR Investigation

## Executive summary

`System.Runtime.CompilerServices.CallSite` and related DLR services are **not** a drop-in replacement for JS2IL's current late-bound member-call pipeline.

They **are** compelling for one narrower slice of the problem: steady-state late-bound invocation of ordinary CLR instance methods. A focused benchmark added during this investigation shows that C# `dynamic` / DLR call sites are dramatically faster than the current `Object.CallMember*` reflection fallback for representative CLR receivers.

The recommended course correction is therefore **narrow, not wholesale**:

- Keep `JavaScriptRuntime.Object.CallMember*` as the semantic authority for JavaScript late-bound member calls.
- Consider a **targeted DLR-backed fast path** only for clearly delimited CLR/host-object fallback cases where the current implementation is reflection-heavy.
- Do **not** migrate JS ordinary objects, delegate-valued members, intrinsic types, or prototype-sensitive dispatch onto the built-in C# dynamic binder.

## Why this investigation matters

There is real appeal in using platform features instead of maintaining hand-written late-bound invocation logic:

- the DLR already provides polymorphic call-site caching,
- it exposes binder hooks for custom rules,
- and it is part of the platform's dynamic invocation story.

However, JS2IL is not solving "late-bound CLR invocation" in the abstract. It is solving **JavaScript/Node-compatible member invocation**. That includes `this` binding, JavaScript primitive behavior, delegate-shaped function values, prototype-sensitive lookup, and Node/V8-style errors. The central question is therefore not "is the DLR faster?" but rather:

> Is the DLR a better implementation vehicle for the parts of late-bound invocation that JS2IL actually needs?

## Current JS2IL late-bound invocation pipeline

### Compiler side

The compiler already avoids generic late-bound dispatch when it can prove a better path:

- `HIRToLIRLowerer.ExpressionCall` emits direct intrinsic or typed instance calls for stable cases such as string `substring`, `JavaScriptRuntime.Array`, `JavaScriptRuntime.Console`, and known user-class instance methods.
- `LIRMemberCallNormalization` rewrites provably safe user-class member calls into `LIRCallTypedMember` or `LIRCallTypedMemberWithFallback`.
- Only the remaining truly dynamic member calls lower to `LIRCallMember0/1/2/3` or `LIRCallMember`.
- `LIRToILCompiler.InstructionEmission.Calls` emits those instructions as calls to `JavaScriptRuntime.Object.CallMember0/1/2/3` or `JavaScriptRuntime.Object.CallMember`.

This means the question is already narrowed: DLR would only compete with the **current dynamic fallback**, not with the compiler's typed fast paths.

### Runtime side

`JavaScriptRuntime.Object.CallMember` is not just "reflection invoke with a string name." It is JS-specific dispatch logic:

- delegates are handled specially for `apply`, `call`, `bind`, and `toString`;
- strings, booleans, and `BigInteger` receivers have special handling;
- `JavaScriptRuntime.Array` uses instance-method dispatch;
- dictionary-backed objects and `ExpandoObject` can expose delegate-valued members;
- delegate invocation explicitly sets the JS `this` value via `RuntimeServices.SetCurrentThis(receiver)`;
- only after those cases does the code fall back to `CallInstanceMethod`.

`CallMember0/1/2/3` avoid some common-case `object[]` allocations, but for CLR receivers they still funnel into `CallMember` and eventually into `CallInstanceMethod`.

### Reflection fallback today

`CallInstanceMethod` is where the current CLR fallback cost comes from:

- it enumerates public instance methods by name;
- it chooses a method using js2il-specific ABI rules (including hidden leading `object[] scopes` and `object newTarget` parameters);
- it applies JS-flavored numeric coercion and argument trimming;
- it sets `this` through `RuntimeServices`;
- and then it invokes the selected `MethodInfo`.

There is **no call-site cache** here today. The current code recomputes method resolution work on each call.

## Existing DLR usage in the repository

The repository already uses DLR-facing APIs, but only at the hosting/interoperability edge:

- `JavaScriptRuntime.Hosting.JsDynamicValueProxy` implements `DynamicObject` and routes `TryInvokeMember` back into `JavaScriptRuntime.ObjectRuntime.CallMember`.
- `JavaScriptRuntime.Hosting.JsDynamicExports` also implements `DynamicObject`, but is specialized for export access and invocation.
- `JavaScriptRuntime.GlobalThis` implements `IDynamicMetaObjectProvider` by delegating to an internal `ExpandoObject` metaobject.

This is an important signal: JS2IL already uses the DLR as a **host-facing convenience layer**, while preserving `CallMember` as the **JS semantics engine** underneath.

## What the DLR gives us

The relevant platform pieces are:

- [`CallSite<T>`](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.callsite-1): dynamic call site with a cached `Target` delegate and an `Update` path for cache misses.
- [`CallSiteBinder`](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.callsitebinder): binder entry point that can provide direct delegates via `BindDelegate`, maintain cached targets, and invalidate bindings via `UpdateLabel`.
- [`DynamicObject`](https://learn.microsoft.com/en-us/dotnet/api/system.dynamic.dynamicobject): base type for custom dynamic behavior. The .NET 10 docs mark it with `RequiresDynamicCode`, which is a deployment consideration for AOT-sensitive scenarios.
- [`IDynamicMetaObjectProvider`](https://learn.microsoft.com/en-us/dotnet/api/system.dynamic.idynamicmetaobjectprovider): interface for objects that want to participate directly in DLR binding.
- [`InvokeMemberBinder`](https://learn.microsoft.com/en-us/dotnet/api/system.dynamic.invokememberbinder): binder surface for dynamic member invocation, including member name and call information.

In short, the DLR offers:

- cached polymorphic dispatch,
- pluggable binding rules,
- a standard platform mechanism instead of bespoke caches,
- and good support for CLR-centric dynamic invocation.

One important nuance: the built-in C# `dynamic` path bakes the member name into the binder/call site. That is useful for source expressions like `receiver.Foo()`, but it is **not** the same shape as `CallMember(receiver, methodName, ...)`, where the method name arrives as a runtime value. Matching `CallMember` more closely requires either:

- a custom `CallSiteBinder` whose delegate signature includes the runtime method name as an argument, or
- a runtime cache of call sites keyed by method name.

## Where the DLR fits well

### 1. CLR host-object fallback

This is the strongest candidate.

For a plain CLR receiver with ordinary instance methods, the DLR's call-site cache is directly aligned with the problem. It can avoid repeated method lookup and can reuse a cached delegate for steady-state calls.

That is exactly the shape of work that `CallInstanceMethod` currently repeats.

### 2. Hosting ergonomics

JS2IL already benefits from `DynamicObject` and `IDynamicMetaObjectProvider` on the hosting side. This is a sign that the platform's dynamic features are useful when the goal is:

- pleasant C# interop,
- dynamic member access from host code,
- or CLR-style dynamic objects.

## Where the DLR does **not** fit as a wholesale replacement

### 1. JavaScript `this` binding

`CallMember` explicitly sets `RuntimeServices.SetCurrentThis(receiver)` before invoking delegate-valued members. That is JS-specific behavior.

The built-in C# dynamic binder does not automatically model JavaScript `this` binding. If JS2IL moved wholesale to DLR member invocation, that behavior would still need custom glue.

### 2. Delegate-valued members and function objects

JS2IL represents many JavaScript function values as CLR delegates plus runtime helpers. `CallMember` knows that a delegate receiver needs special handling for `apply`, `call`, `bind`, and `toString`.

The C# dynamic binder does not know any of those JavaScript conventions. Replacing `CallMember` here would lose semantics unless JS2IL recreated them in custom binders or metaobjects.

### 3. Primitive and intrinsic special cases

`CallMember` has JavaScript-aware behavior for:

- string-like receivers,
- booleans,
- `BigInteger`,
- `JavaScriptRuntime.Array`,
- dictionary-backed objects,
- `ExpandoObject`,
- and host objects exposing delegate-valued members.

The built-in DLR binder is a CLR member binder, not a JavaScript semantic dispatcher. It would not preserve these behaviors automatically.

### 4. Prototype-sensitive lookup and mutation invalidation

Existing design docs already point toward a JS-specific cache keyed by shape / prototype-version information.

The DLR **can** support invalidation in principle: `CallSiteBinder.UpdateLabel` exists for rebinding when object "versions" change. But making that work for JS2IL would require custom binders or custom metaobjects that understand:

- prototype-chain lookup,
- invalidation on prototype mutation,
- own-property vs inherited-property resolution,
- and the receiver kinds JS2IL currently uses.

At that point, the implementation would no longer be "use the built-in wheel." It would be "build a JS-specific dynamic object system on top of the DLR."

### 5. js2il-specific ABI handling

`CallInstanceMethod` knows about js2il's hidden `scopes` and `newTarget` ABI details, and it performs JS-oriented coercion and argument trimming. The built-in CLR binder does not.

A DLR replacement would need those rules reimplemented somewhere.

### 6. Dynamic-code/AOT considerations

`DynamicObject` is marked `RequiresDynamicCode` in the .NET docs. That does not automatically disqualify DLR use in JS2IL, but it is a real trade-off:

- it introduces pressure toward runtime code generation,
- and it is a mismatch with any future deployment scenarios that care about ahead-of-time restrictions.

Given that JS2IL is itself an AOT compiler, this is worth treating as a design cost, not a footnote.

## Benchmark evidence gathered for this investigation

To avoid relying only on code inspection, this investigation added a research-only benchmark:

- benchmark class: `tests/performance/Benchmarks/LateBoundDispatchBenchmarks.cs`
- run command: `dotnet run -c Release --project .\tests\performance\Benchmarks\Benchmarks.csproj -- --dispatch`

The benchmark intentionally focuses on **representative CLR receiver dispatch**, because that is the part of the problem where DLR has the strongest chance to help.

Important caveats:

- it measures **steady-state** behavior after a `GlobalSetup` warmup primes the call sites;
- it does **not** measure cold-start binder cost;
- it does **not** prove semantic equivalence for JavaScript objects;
- it does **not** benchmark prototype-sensitive lookup.

### Summary results

| Case | Mean | Allocated | Notes |
| --- | ---: | ---: | --- |
| DLR dynamic CLR receiver (0 args) | 15.0275 ns | 24 B | C# `dynamic`; member name fixed at call site |
| DLR dynamic CLR receiver (1 arg) | 14.1022 ns | 24 B | C# `dynamic`; member name fixed at call site |
| DLR CallSite runtime method name (0 args) | 9.153 ns | 24 B | Custom `CallSiteBinder`; method name passed at runtime |
| DLR CallSite runtime method name (1 arg) | 14.765 ns | 48 B | Custom `CallSiteBinder`; method name passed at runtime |
| CallMember0 CLR receiver | 373.5117 ns | 552 B | Current reflection-heavy fallback |
| CallMember1 CLR receiver | 449.4950 ns | 744 B | Current reflection-heavy fallback |

### Interpretation of the benchmark

For these CLR-receiver cases:

- fixed-name DLR (`dynamic`) was roughly **25x to 32x faster** than the current `CallMember*` fallback;
- runtime-name DLR via a custom `CallSiteBinder` was still roughly **30x to 41x faster** than the current `CallMember*` fallback;
- runtime-name DLR allocated roughly **15x to 23x less memory** than the current reflection-based path.

This is strong evidence that the current `CallInstanceMethod` fallback is leaving real performance on the table for CLR receivers.

It is also evidence that the earlier "fixed-name `dynamic`" comparison was only a partial story: once the benchmark was adjusted so the method name arrived at runtime, the DLR still looked very good for CLR fallback work.

It is **not** evidence that the DLR should replace the whole JS member-call pipeline.

## Practical design options

### Option A: Replace `CallMember` wholesale with the built-in DLR binder

**Assessment:** Not recommended.

Why:

- semantics mismatch is too large;
- JS-specific behavior would still need custom machinery;
- prototype/version handling would still be a JS-specific system;
- existing typed fast paths would gain little from such a rewrite;
- the change would be invasive across compiler lowering, runtime objects, and invalidation design.

### Option B: Keep `CallMember`, add a JS-aware cache inside runtime helpers

**Assessment:** Still the best general-purpose path for JavaScript semantics.

Why:

- it matches the repository's current shape/prototype-version design direction;
- it naturally supports shape-attached cached callsites or inline caches keyed by receiver shape and prototype version;
- it keeps semantics centralized in runtime helpers;
- it can be tailored to Node/V8-compatible behavior;
- and it does not require the object model to become DLR-native.

### Option C: Add a narrow DLR-backed fast path for CLR host-object fallback

**Assessment:** Worth serious consideration.

This is the main self-correction coming out of the investigation.

If the real performance pain is late-bound invocation against ordinary CLR receivers, then the DLR is not just philosophically appealing; it has benchmark evidence behind it even when the method name is only known at runtime.

The natural target would be a narrow fast path around the **current reflection fallback**, not a rewrite of JS dispatch as a whole. That fast path would most likely use a **custom binder/call-site strategy**, not just the built-in C# `dynamic` binder.

## Recommendation

1. **Do not replace `JavaScriptRuntime.Object.CallMember*` wholesale with DLR-based invocation.**
   The semantic gap is too large, and the built-in C# dynamic binder is not a JavaScript invocation engine.

2. **Treat DLR as a candidate optimization for CLR/host-object fallback only.**
   The benchmark results justify further investigation here, including the runtime-method-name case that matches `CallMember` more closely.

3. **Preserve `CallMember` as the semantic hub.**
   If a DLR-backed path is introduced, it should sit behind explicit receiver-kind checks and fall back to existing runtime helpers for JS semantics.

4. **If DLR is explored further, frame it as a custom binder/call-site cache for CLR fallback, not as "just use `dynamic`."**
   The runtime-name benchmark shows that a custom binder can still be fast, but it also means some custom infrastructure work is unavoidable.

5. **Keep JS-wide caching work custom unless a custom DLR binder proves simpler and faster.**
   For prototype-aware JavaScript dispatch, the existing shape/version-cache direction still looks like the better fit, especially if callsites can be cached against stable receiver shapes.

## Suggested next steps

If this investigation turns into follow-up work, the next steps should be:

1. Prototype a **CLR-receiver-only** fast path near `CallInstanceMethod`.
2. Benchmark both **steady-state and cold-start** behavior.
3. Decide whether the prototype should use:
   - one custom call site whose binder takes `methodName` as a runtime argument, or
   - a site cache keyed by method name and arity.
4. Add targeted tests that verify the fast path does not break:
   - js2il ABI handling,
   - JS numeric coercion expectations where relevant,
   - `this` handling for delegate-valued members,
   - and fallback behavior when the receiver is not an ordinary CLR method target.

## Evidence used

- `Compiler/IR/LIR/HIRToLIRLowerer.ExpressionCall.cs`
- `Compiler/IR/LIR/LIRMemberCallNormalization.cs`
- `Compiler/IL/LIRToILCompiler.InstructionEmission.Calls.cs`
- `Compiler/IL/LIRToILCompiler.TypedCalls.cs`
- `JavaScriptRuntime/Object.cs`
- `JavaScriptRuntime/ObjectRuntime.cs`
- `JavaScriptRuntime/Hosting/JsDynamicValueProxy.cs`
- `JavaScriptRuntime/Hosting/JsDynamicExports.cs`
- `JavaScriptRuntime/GlobalThis.cs`
- `docs/compiler/Prototypes_SupportDesign.md`
- `docs/compiler/PrototypeChainSupport.md`
- Local benchmark runs generated from `tests/performance/Benchmarks/LateBoundDispatchBenchmarks.cs` via `dotnet run -c Release --project .\tests\performance\Benchmarks\Benchmarks.csproj -- --dispatch`

