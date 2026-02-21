<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 9.5: Jobs and Host Operations to Enqueue Jobs

[Back to Section9](Section9.md) | [Back to Index](../Index.md)

| Clause | Title | Status | Link |
|---:|---|---|---|
| 9.5 | Jobs and Host Operations to Enqueue Jobs | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-jobs) |

## Subclauses

| Clause | Title | Status | Spec |
|---:|---|---|---|
| 9.5.1 | JobCallback Records | Supported | [tc39.es](https://tc39.es/ecma262/#sec-jobcallback-records) |
| 9.5.2 | HostMakeJobCallback ( callback ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-hostmakejobcallback) |
| 9.5.3 | HostCallJobCallback ( jobCallback , V , argumentsList ) | Supported | [tc39.es](https://tc39.es/ecma262/#sec-hostcalljobcallback) |
| 9.5.4 | HostEnqueueGenericJob ( job , realm ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-hostenqueuegenericjob) |
| 9.5.5 | HostEnqueuePromiseJob ( job , realm ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-hostenqueuepromisejob) |
| 9.5.6 | HostEnqueueTimeoutJob ( timeoutJob , realm , milliseconds ) | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-hostenqueuetimeoutjob) |

## Support

Feature-level support tracking with test script references.

### 9.5.1 ([tc39.es](https://tc39.es/ecma262/#sec-jobcallback-records))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| JobCallback Records | Supported | [`Promise_Resolve_Then.js`](../../../Js2IL.Tests/Promise/JavaScript/Promise_Resolve_Then.js)<br>[`Promise_Scheduling_StarvationTest.js`](../../../Js2IL.Tests/Promise/JavaScript/Promise_Scheduling_StarvationTest.js) | Implemented as internal JobCallback records carrying callback plus host-defined payload slot. |

### 9.5.2 ([tc39.es](https://tc39.es/ecma262/#sec-hostmakejobcallback))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| HostMakeJobCallback | Supported | [`Promise_Resolve_Then.js`](../../../Js2IL.Tests/Promise/JavaScript/Promise_Resolve_Then.js) | Host operation wraps callbacks in JobCallbackRecord for queueing/invocation. |

### 9.5.3 ([tc39.es](https://tc39.es/ecma262/#sec-hostcalljobcallback))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| HostCallJobCallback | Supported | [`Promise_Resolve_Then.js`](../../../Js2IL.Tests/Promise/JavaScript/Promise_Resolve_Then.js) | Host operation invokes stored job callback; JS2IL currently uses Action-based jobs for Promise reactions. |

### 9.5.4 ([tc39.es](https://tc39.es/ecma262/#sec-hostenqueuegenericjob))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| HostEnqueueGenericJob | Supported with Limitations | [`Promise_Scheduling_StarvationTest.js`](../../../Js2IL.Tests/Promise/JavaScript/Promise_Scheduling_StarvationTest.js)<br>[`Process_NextTick_And_Promise_Ordering.js`](../../../Js2IL.Tests/Node/Process/JavaScript/Process_NextTick_And_Promise_Ordering.js) | Generic host job enqueuing is modeled through NodeSchedulerState/EventLoopPump queues; realm-specific behavior is limited. |

### 9.5.5 ([tc39.es](https://tc39.es/ecma262/#sec-hostenqueuepromisejob))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| HostEnqueuePromiseJob | Supported with Limitations | [`Promise_Resolve_Then.js`](../../../Js2IL.Tests/Promise/JavaScript/Promise_Resolve_Then.js)<br>[`Promise_Scheduling_StarvationTest.js`](../../../Js2IL.Tests/Promise/JavaScript/Promise_Scheduling_StarvationTest.js) | Promise reactions enqueue microtasks via IMicrotaskScheduler and are drained by the event-loop microtask checkpoints. |

### 9.5.6 ([tc39.es](https://tc39.es/ecma262/#sec-hostenqueuetimeoutjob))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| HostEnqueueTimeoutJob | Supported with Limitations | [`SetTimeout_ZeroDelay.js`](../../../Js2IL.Tests/Node/Timers/JavaScript/SetTimeout_ZeroDelay.js)<br>[`SetInterval_ExecutesThreeTimes_ThenClears.js`](../../../Js2IL.Tests/Node/Timers/JavaScript/SetInterval_ExecutesThreeTimes_ThenClears.js) | Timers enqueue timeout jobs through scheduler timer queues (setTimeout/setInterval) with host-specific event loop behavior. |

## Reference: Converted Spec Text

_Intentionally not included here. Use the tc39.es links above as the normative reference._

