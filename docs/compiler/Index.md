# Compiler design

These design documents describe the JROC compiler pipeline, including callable planning, lowering, IL emission, JavaScript semantics, module resolution, debug information, and optimization work. They are implementation references rather than end-user guides.

## Compilation architecture and callable model

- [Callable architecture baselines](CallableArchitectureBaselines.md)
- [Captured variables and the scopes ABI](CapturedVariables_ScopesABI.md)
- [Generated arrow function objects](GeneratedArrowFunctionObjects.md)
- [Generated function-object types](GeneratedFunctionObjectTypes.md)
- [Stable function-valued binding calls](StableFunctionBindingCalls.md)
- [Two-phase compilation pipeline](TwoPhaseCompilationPipeline.md)

## Lowering, code generation, and optimization

- [Async/await lowering specification](AsyncAwait_LoweringSpec.md)
- [Async/await three-way comparison](AsyncAwait_ThreeWay_Comparison.md)
- [Guarded String intrinsic calls](GuardedStringIntrinsicCalls.md)
- [Instruction chaining](InstructionChaining.md)
- [LIR rematerialization](LIRRematerialization.md)
- [LIR stack scheduler](LIRStackScheduler.md)
- [Portable PDB emission plan](PdbEmission_Plan.md)
- [Synchronous generator lowering specification](SynchronousGenerators_LoweringSpec.md)

## JavaScript semantics and object model

- [JavaScript `eval` support design](EvalSupportDesign.md)
- [JavaScript to .NET type mapping](JavaScriptToDotNetTypeMapping.md)
- [Object literal type inference](ObjectLiteralTypeInference.md)
- [Prototype chain support strategy](PrototypeChainSupport.md)
- [Prototype support design](Prototypes_SupportDesign.md)

## Modules, invocation, and asynchronous execution

- [Event loop and scheduling](EventLoopAndScheduling.md)
- [Late-bound invocation and DLR investigation](LateBoundInvocation_DLR_Investigation.md)
- [npm package imports](NpmPackageImports.md)
- [Pending IO count for `fs/promises.readFile`](PendingIOCount_ReadFileAsync_Design.md)
- [Scopes, classes, and async/generator state](Scopes_Classes_AsyncGenerator_Design.md)
