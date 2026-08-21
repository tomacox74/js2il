# Realm-created value caches

`RuntimeRealmValueCacheState` owns caches whose entries contain JavaScript
values or captured scope objects:

- tagged-template arrays, keyed by compiled call-site ID;
- materialized class constructor objects, keyed by generated CLR type, formal
  parameter count, and captured scope identities;
- dynamic lookup inline-cache sites, keyed by generated call-site ID, with
  weak receiver, value, and prototype references;
- lazy class-method metadata that includes the captured scopes needed when a
  method function is first materialized.

Generated CLR types, method handles, and other immutable reflection metadata
may remain process-wide. A JavaScript object, callback wrapper, mutable
descriptor, realm prototype, or captured scope must not be stored in a
process-wide cache.

Repeated evaluation in one realm preserves the identities required by
ECMA-262. Another realm executing the same generated code gets independent
template and constructor identities, even when it uses the same call-site ID,
generated type, and scope objects.

The cache state is created and disposed with `RuntimeRealm`. Disposal clears
all caches so cached JavaScript values, captured scopes, and collectible
generated assemblies can be reclaimed. Dynamic lookup entries also use weak
references so they do not extend object lifetimes while the realm is active.
