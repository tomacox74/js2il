# Runtime design

These documents describe the JavaScriptRuntime hosting model and its internal execution, object, lifecycle, ownership, realm, scheduling, and performance behavior. Use the SDK documentation for supported consumer-facing hosting workflows.

## Hosting and invocation

- [Hosting compiled JavaScript as a .NET library](DotNetLibraryHosting.md)
- [JsFunctionObject invocation ABI](JsFunctionObjectInvocationAbi.md)

## Object model and execution state

- [Runtime object representation](OrdinaryObjectRepresentation.md)
- [Runtime execution frames](RuntimeExecutionFrames.md)
- [Runtime module state](RuntimeModuleState.md)
- [Realm-created value caches](RuntimeRealmValueCaches.md)

## Lifecycle and scheduling

- [Runtime agent scheduling](RuntimeAgentScheduling.md)
- [Runtime lifecycle](RuntimeLifecycle.md)
- [Runtime ownership](RuntimeOwnership.md)

## Performance

- [RegExp and string hot-path performance optimizations](RegExpStringPerformanceOptimizations.md)
