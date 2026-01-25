<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 9.5: Jobs and Host Operations to Enqueue Jobs

[Back to Section9](Section9.md) | [Back to Index](../Index.md)

| Clause | Title | Status | Link |
|---:|---|---|---|
| 9.5 | Jobs and Host Operations to Enqueue Jobs | Supported | [tc39.es](https://tc39.es/ecma262/#sec-jobs) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 9.5.1 | JobCallback Records | Supported | [tc39.es](https://tc39.es/ecma262/#sec-jobcallback-records) |
| 9.5.2 | HostMakeJobCallback ( callback ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-hostmakejobcallback) |
| 9.5.3 | HostCallJobCallback ( jobCallback , V , argumentsList ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-hostcalljobcallback) |
| 9.5.4 | HostEnqueueGenericJob ( job , realm ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-hostenqueuegenericjob) |
| 9.5.5 | HostEnqueuePromiseJob ( job , realm ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-hostenqueuepromisejob) |
| 9.5.6 | HostEnqueueTimeoutJob ( timeoutJob , realm , milliseconds ) | Untracked | [tc39.es](https://tc39.es/ecma262/#sec-hostenqueuetimeoutjob) |

## Support

Feature-level support tracking with test script references.

### 9.5.1 ([tc39.es](https://tc39.es/ecma262/#sec-jobcallback-records))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| JobCallback Records | Supported | [`Promise_Resolve_Then.js`](../../../Js2IL.Tests/Promise/JavaScript/Promise_Resolve_Then.js)<br>[`Promise_Scheduling_StarvationTest.js`](../../../Js2IL.Tests/Promise/JavaScript/Promise_Scheduling_StarvationTest.js) | Implemented as an internal JobCallback record carrying a callback plus a host-defined context slot (currently null/empty). Used by Promise reaction job scheduling so future host-defined context/realm data can be threaded without changing observable behavior. |

### 9.5.2 ([tc39.es](https://tc39.es/ecma262/#sec-hostmakejobcallback))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| HostMakeJobCallback | Supported | [`Promise_Resolve_Then.js`](../../../Js2IL.Tests/Promise/JavaScript/Promise_Resolve_Then.js)<br>[`Promise_Scheduling_StarvationTest.js`](../../../Js2IL.Tests/Promise/JavaScript/Promise_Scheduling_StarvationTest.js) | Default host behavior: wraps a callback into a JobCallback record with an empty HostDefined field. |

### 9.5.3 ([tc39.es](https://tc39.es/ecma262/#sec-hostcalljobcallback))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| HostCallJobCallback | Supported | [`Promise_Resolve_Then.js`](../../../Js2IL.Tests/Promise/JavaScript/Promise_Resolve_Then.js)<br>[`Promise_Scheduling_StarvationTest.js`](../../../Js2IL.Tests/Promise/JavaScript/Promise_Scheduling_StarvationTest.js) | Default host behavior: invokes the stored callback. Parameters (V, argumentsList) are accepted for spec-shape compatibility; JS2IL currently models Promise jobs as parameterless Actions. |

### 9.5.4 ([tc39.es](https://tc39.es/ecma262/#sec-hostenqueuegenericjob))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| HostEnqueuePromiseJob | Supported | [`Promise_Resolve_Then.js`](../../../Js2IL.Tests/Promise/JavaScript/Promise_Resolve_Then.js)<br>[`Promise_Scheduling_StarvationTest.js`](../../../Js2IL.Tests/Promise/JavaScript/Promise_Scheduling_StarvationTest.js) | Promise reaction jobs are enqueued onto the runtime microtask queue (IMicrotaskScheduler) and executed by the event loop pump. |

## Reference: Converted Spec Text

_Intentionally not included here. Use the tc39.es links above as the normative reference._

