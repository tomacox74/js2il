<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 9.8: Forward Progress

[Back to Section9](Section9.md) | [Back to Index](../Index.md)

| Clause | Title | Status | Link |
|---:|---|---|---|
| 9.8 | Forward Progress | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-forward-progress) |

## Support

Feature-level support tracking with test script references.

### 9.8 ([tc39.es](https://tc39.es/ecma262/#sec-forward-progress))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Forward progress in event-loop/microtask scheduling | Supported with Limitations | [`Promise_Scheduling_StarvationTest.js`](../../../Js2IL.Tests/Promise/JavaScript/Promise_Scheduling_StarvationTest.js)<br>[`Hosting_EventLoopKeepAlive.js`](../../../Js2IL.Tests/Hosting/JavaScript/Hosting_EventLoopKeepAlive.js) | NodeEventLoopPump bounds work per checkpoint and drains queued work deterministically for supported host queues; full multi-agent progress guarantees are out of scope. |

