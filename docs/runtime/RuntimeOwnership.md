# Runtime ownership

JROC runtime state has three explicit lifetime owners:

```text
RuntimeAgentCluster
  -> RuntimeAgent
      -> RuntimeRealm
          -> ServiceContainer
          -> RuntimeExecutionContext (while entered)
```

- `RuntimeAgentCluster` owns agents and, in later migration stages, only
  resources intentionally shared across agents.
- `RuntimeAgent` owns execution and scheduling lifecycle and can own one or
  more realms.
- `RuntimeRealm` owns one JavaScript object graph and exactly one runtime
  `ServiceContainer`.

The child keeps a reference to its parent so services can receive the correct
owner through constructor injection. Parents keep their children only while
the children are active. Disposing a child detaches it from its parent.
Disposing a parent disposes children in reverse creation order, then marks the
parent disposed. Disposal is idempotent.

Runtime service containers register their realm, agent, and cluster as reserved
services. Those registrations cannot be replaced or removed. Child DI scopes
retain the same realm owner. After realm disposal, its service container and
any child scopes reject further use.

The factory currently creates an isolated cluster, agent, realm, and service
container for each `RuntimeServices.BuildServiceProvider()` call. Existing
globals, modules, schedulers, caches, ambient execution state, `Engine`, and
hosting lifecycle still use their prior implementations; subsequent migration
issues move that state under these owners.

## Reference rules

- Process-wide code may retain immutable metadata, but not an active realm,
  agent, or cluster.
- A cluster may reference its active agents.
- An agent may reference its cluster and active realms.
- A realm may reference its agent and service container.
- An entered execution context is the only ambient pointer to a realm and
  agent; thread identity is not an ownership boundary.
- Realm services may receive any of the three owners through constructor
  injection, but must not discover them through a second service locator.
- Realm-created JavaScript objects must not be stored on an agent or cluster.
- Cross-agent resources must not retain realm-created wrappers or callbacks.
