<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 9.3: Realms

[Back to Section9](Section9.md) | [Back to Index](../Index.md)

> Last generated (UTC): 2026-10-25T02:53:21Z

Host-created RuntimeRealm instances own separate global objects, intrinsic graphs, descriptors and module state. One agent can own multiple realms. Coverage is bounded by the implemented intrinsic and host entry points rather than a complete general host realm-control API.

| Clause | Title | Status | Link |
|---:|---|---|---|
| 9.3 | Realms | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-code-realms) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 9.3.1 | InitializeHostDefinedRealm ( ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-initializehostdefinedrealm) |
| 9.3.2 | CreateIntrinsics ( realmRec ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-createintrinsics) |
| 9.3.3 | SetDefaultGlobalBindings ( realmRec ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-setdefaultglobalbindings) |

## Support

Feature-level support tracking with repo test references and optional test262 evidence.

### 9.3.1 ([tc39.es](https://tc39.es/ecma262/#sec-initializehostdefinedrealm))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| Host-defined realm initialization and ownership | Supported with Limitations | `tests/Jroc.Tests/RuntimeOwnershipTests.cs`<br>`tests/Jroc.Tests/RuntimeExecutionContextTests.cs` |  | RuntimeLifecycle creates or joins an explicit agent-cluster/agent/realm ownership graph. Agents may own multiple realms; entered execution frames select and restore the current realm, including exception unwinding and asynchronous continuation paths. A complete general JavaScript host realm-control API is not implied. |

### 9.3.2 ([tc39.es](https://tc39.es/ecma262/#sec-createintrinsics))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| CreateIntrinsics for realm-owned intrinsic graphs | Supported with Limitations | `tests/Jroc.Tests/RuntimeIntrinsicIsolationTests.cs`<br>`tests/Jroc.Tests/RuntimeIntrinsicConcurrencyTests.cs` |  | Every realm owns its intrinsic prototypes, built-in function identities and descriptor baseline. Lazy initialization preserves identity within a realm and isolates mutations across realms, including supported cross-realm constructor prototype fallback. This describes the implemented intrinsic surface, not support for every ECMA-262 built-in. |

### 9.3.3 ([tc39.es](https://tc39.es/ecma262/#sec-setdefaultglobalbindings))

| Feature name | Status | Test scripts | test262 evidence | Notes |
|---|---|---|---|---|
| SetDefaultGlobalBindings | Supported with Limitations | [`GlobalTimers_AsValues_WindowLikeAssignment.js`](../../../tests/Jroc.Tests/Node/Timers/JavaScript/GlobalTimers_AsValues_WindowLikeAssignment.js)<br>`tests/Jroc.Tests/RuntimeIntrinsicIsolationTests.cs`<br>`tests/Jroc.Tests/RuntimeModuleStateTests.cs` |  | GlobalThis bootstraps supported default bindings against the owning realm's intrinsic graph. Realms keep distinct globals, descriptors, module bindings and namespace caches; subsequent user mutations are not overwritten by repeated bootstrap. |

