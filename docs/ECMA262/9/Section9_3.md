<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 9.3: Realms

[Back to Section9](Section9.md) | [Back to Index](../Index.md)

| Clause | Title | Status | Link |
|---:|---|---|---|
| 9.3 | Realms | Incomplete | [tc39.es](https://tc39.es/ecma262/#sec-code-realms) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 9.3.1 | InitializeHostDefinedRealm ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-initializehostdefinedrealm) |
| 9.3.2 | CreateIntrinsics ( realmRec ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-createintrinsics) |
| 9.3.3 | SetDefaultGlobalBindings ( realmRec ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-setdefaultglobalbindings) |

## Support

Feature-level support tracking with test script references.

### 9.3.1 ([tc39.es](https://tc39.es/ecma262/#sec-initializehostdefinedrealm))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Host-defined realm initialization (single runtime realm) | Supported with Limitations | [`Hosting_EventLoopKeepAlive.js`](../../../Js2IL.Tests/Hosting/JavaScript/Hosting_EventLoopKeepAlive.js) | Engine bootstrap initializes one host realm/global runtime context per execution instance; multi-realm semantics are limited. |

### 9.3.2 ([tc39.es](https://tc39.es/ecma262/#sec-createintrinsics))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| CreateIntrinsics for runtime global object | Supported with Limitations | `Js2IL.Tests/ValidatorTests.cs`<br>[`Process_Platform_Versions_And_Env_Basics.js`](../../../Js2IL.Tests/Node/Process/JavaScript/Process_Platform_Versions_And_Env_Basics.js) | GlobalThis is populated with runtime intrinsics/host APIs, but does not fully model per-realm intrinsic identity across multiple realms. |

### 9.3.3 ([tc39.es](https://tc39.es/ecma262/#sec-setdefaultglobalbindings))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| SetDefaultGlobalBindings | Supported with Limitations | [`GlobalTimers_AsValues_WindowLikeAssignment.js`](../../../Js2IL.Tests/Node/Timers/JavaScript/GlobalTimers_AsValues_WindowLikeAssignment.js) | Default global bindings are established through GlobalThis initialization for the supported host/runtime surface. |

