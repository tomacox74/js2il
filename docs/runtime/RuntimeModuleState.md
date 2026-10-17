# Runtime module state

Each `RuntimeRealm` owns exactly one `RuntimeModuleState`. It is the lifetime
boundary for all mutable CommonJS and ECMAScript module graph state:

- CommonJS module instances, exports caches, parent/main relationships, and
  Node module instances;
- the compiled modules assembly and its lazily discovered module type map;
- module-scoped require delegates used by dynamic import;
- `import.meta` objects keyed by canonical URL;
- ESM live binding cells, native namespace markers, and cached namespace
  exotic objects;
- synthesized CommonJS namespace objects;
- deferred ESM namespace identities and JSON/text import namespace identities;
- lazily read compiled-module metadata for static dependencies, top-level
  await, export names, deferred loading errors, and original JSON source.

Identical canonical module IDs can therefore exist concurrently in different
realms without sharing exports, bindings, namespace identity, or
`import.meta`. Immutable compiled metadata and the Node module type registry
may still be discovered process-wide, but realm-created values are stored only
in `RuntimeModuleState`.

Module filename, directory, and the active CommonJS parent module belong to
the current execution frame. Nested frame entry inherits those values and
restores the previous values when its scope exits. Root entry starts without
an active parent module.

Disposing a realm clears its module state as one operation. Runtime shutdown
does not unregister entries from process-wide dictionaries because no mutable
module graph is process-owned.

Ordinary and deferred namespace objects have separate identities. Deferred
namespaces evaluate their synchronous target on string-key operations, except
the special `then` key; symbol operations do not trigger evaluation. Their
asynchronous transitive dependencies finish before the import promise resolves.
Evaluation failures remain attached to the realm's ESM module record so later
imports reject with the same JavaScript error value rather than retrying the
module body. Namespace and compiled-metadata caches are cleared with the realm.
