# Futures

This file captures forward-looking ideas discussed for JS2IL strategy and roadmap.

## Product Positioning

JS2IL should not try to out-V8 V8 on raw peak performance in the short term.  
Its strongest differentiator is **.NET-native JavaScript execution and hosting**:

- AOT-compiled JavaScript to .NET assemblies
- no native V8 dependency required
- strong C# interoperability and platform integration
- enterprise-friendly deployment, observability, and governance

## Priority Order (near-to-mid term)

1. **Node intrinsic compatibility + high-value ECMA-262 coverage**
2. **Hosting DX (developer experience for embedding JS in .NET)**
3. **Performance optimization in focused hot paths (continuous track)**
4. Broader ecosystem polish after the above are solid

Rationale: adoption is driven first by "can it run my workload?" and "is it easy to embed/operate?" then by absolute speed.

## Concrete Goals Beyond Compatibility

### 1) Hosting DX

Ship a minimal, delightful host experience:

- one NuGet + small host boilerplate
- easy binding of .NET services into JavaScript
- async + cancellation integration
- predictable value conversion between JS and C#
- clear errors/diagnostics for host developers

### 2) Ops & Governance

- first-class `ILogger` + OpenTelemetry integration
- execution constraints (timeouts, memory limits, module allowlists)
- deterministic, auditable runtime behavior suitable for enterprise services

OpenTelemetry should be treated as a flagship differentiator:

- built-in trace/span propagation across JS and .NET calls
- consistent correlation with ASP.NET Core request/activity pipelines
- simple export wiring for enterprise observability stacks
- benchmarking/profiling hooks to explain JS2IL runtime behavior in production

### 3) Deployment Economics

- compile-to-memory execution path
- optional persistent cache of generated assembly bytes
- Node-free distribution options and clear cold-start/memory comparisons

## Compatibility Strategy for .NET-Only Power Features

Concern: exposing .NET-only APIs (for example EF Core) can create a surface that does not run on Node.js.

Proposed strategy:

- keep **core runtime APIs** Node-compatible by default
- add **opt-in .NET extension modules** for platform-specific power features
- avoid polluting the global/common API surface with .NET-specific contracts
- clearly document portability tiers (`node-compatible`, `js2il-dotnet-extension`)
- provide fallback/adapter guidance so teams can isolate non-portable code paths

Example direction:

- `import { db } from 'js2il/dotnet/efcore'` (explicitly non-portable extension)
- keep app/business logic in portable modules where possible

## PrimeJavaScript Performance Program

Track under the Prime epic and sub-issues:

- #738 (parent)
- #737, #739, #740, #741, #742, #743 (sub-issues)

Main optimization themes:

- guarded direct dispatch for internal hot calls (avoid dynamic `CallMember*` where safe)
- keep loop math in typed locals (`int32`/`double`) with conservative fallback
- reduce per-iteration allocation/invocation overhead in batch paths
- trim timing/config coercion overhead
- require benchmark evidence per change (throughput + allocation deltas)

## In-Memory Compilation, Caching, REPL Direction

- Add `CompileToMemory` style API returning assembly bytes/stream
- load with `AssemblyLoadContext.LoadFromStream`
- support persistent cache keyed by source hash + compiler version + options
- explore REPL/session model with persistent host state across cells

## Glossary

### Hosting DX

**Hosting DX** means the day-to-day developer experience of embedding and operating JS2IL inside a .NET app:

- how easy it is to set up and call
- how predictable value marshaling and async behavior are
- how easy it is to debug, observe, and secure
- how much glue code is required to achieve production use cases

In short: *how fast and confidently a .NET developer can ship with JS2IL as a hosted engine/compiler component*.

