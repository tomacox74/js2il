# ADR 0001: DI Container Choice (Runtime vs Compiler)

- Date: 2025-12-21
- Status: Accepted

## Context

This repository contains two distinct components with different constraints:

- **JavaScriptRuntime**: a runtime library that is shipped alongside generated assemblies and is loaded when executing compiled JavaScript.
- **Js2IL (compiler)**: a compile-time toolchain that runs during compilation to generate .NET assemblies.

We need a dependency injection (DI) mechanism for:

- centralizing singleton construction and overrides (e.g., tests)
- constructor injection
- replacing singletons with mocks

However, the runtime component has a strong requirement to avoid taking additional dependencies that would become part of the runtime surface area.

More specifically, the goal is that any assembly produced by Js2IL has runtime dependencies on:

- **.NET / base class libraries**, and
- **JavaScriptRuntime**

…and nothing else. This keeps distribution/deployment simple (only the compiled assembly + JavaScriptRuntime are required) and avoids runtime dependency/version conflicts caused by additional transitive packages.

## Decision

- **JavaScriptRuntime uses a small hand-rolled DI container** (`ServiceContainer`) with:
  - singleton-only semantics
  - constructor injection
  - ability to replace instances for tests
  - automatic creation of dependencies on demand

- **Js2IL (compiler) uses Microsoft.Extensions.DependencyInjection** for compile-time services.

This is an explicit split: the runtime optimizes for minimal dependency footprint; the compiler optimizes for maintainability and ecosystem compatibility.

## Consequences

### Positive

- **JavaScriptRuntime remains dependency-light**: compiled outputs only need JavaScriptRuntime + .NET at runtime, which keeps distribution/deployment simple and avoids dependency/version conflicts from extra transitive packages.
- **Js2IL uses a well-supported DI ecosystem**: common patterns, predictable behavior, and reduced need to maintain custom container logic for compile-time services.
- **Testing remains straightforward** in both layers:
  - runtime: replace singletons in `ServiceContainer`
  - compiler: use the standard DI registration/override patterns

### Negative

- There are **two DI implementations** to understand.
- Patterns and APIs are **not identical** between runtime and compiler.

### Mitigations

- Keep the runtime container intentionally minimal (singletons + constructor injection only).
- Prefer using the narrowest required “service locator” pattern at call sites (e.g., request dependencies through constructors rather than grabbing from the container), so switching containers later is low-cost.

## Alternatives Considered

### 1) Use Microsoft.Extensions.DependencyInjection everywhere

Rejected because it would add an external dependency to JavaScriptRuntime, which conflicts with the goal of keeping runtime dependencies minimal.

### 2) Use the hand-rolled container everywhere

Rejected because it would increase the maintenance burden in Js2IL (compile-time) without a corresponding benefit; the compiler has fewer constraints and benefits from using the Microsoft-supported container.

### 3) Introduce a shared minimal abstraction (adapter)

Possible future refinement: define a tiny interface (resolve/register/replace singletons) and provide adapters for both containers. This can increase consistency while keeping the runtime dependency footprint low.
