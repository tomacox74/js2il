<!-- AUTO-GENERATED: generateEcma262SectionMarkdown.js -->

# Section 9.7: Agent Clusters

[Back to Section9](Section9.md) | [Back to Index](../Index.md)

JS2IL currently runs with a single host/runtime agent model, effectively treating execution as one agent cluster.

| Clause | Title | Status | Link |
|---:|---|---|---|
| 9.7 | Agent Clusters | Supported with Limitations | [tc39.es](https://tc39.es/ecma262/#sec-agent-clusters) |

## Support

Feature-level support tracking with test script references.

### 9.7 ([tc39.es](https://tc39.es/ecma262/#sec-agent-clusters))

| Feature name | Status | Test scripts | Notes |
|---|---|---|---|
| Single-cluster host execution model | Supported with Limitations | [`Process_NextTick_And_Promise_Ordering.js`](../../../Js2IL.Tests/Node/Process/JavaScript/Process_NextTick_And_Promise_Ordering.js) | No multi-agent shared-memory cluster semantics are exposed; current runtime assumes one cluster context. |

