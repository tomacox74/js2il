# Node contract generation

Node module contracts are generated manually from the pinned official Node.js
documentation. Generated contracts are not checked on every CI run because the
inputs are intentionally stable.

Regenerate and inspect the contracts when:

- the contract specification changes;
- a JavaScript-to-.NET type mapping changes;
- the generator changes; or
- the pinned Node.js version changes.

For the `node:fs` proof of concept:

```sh
npm run generate:node-contract-fs
npm run check:node-contract-fs
npm run test:node-contract-fs
```

For `node:fs/promises`:

```sh
npm run generate:node-contract-fs-promises
npm run check:node-contract-fs-promises
npm run test:node-contract-fs-promises
```

For `node:console`:

```sh
npm run generate:node-contract-console
npm run check:node-contract-console
npm run test:node-contract-console
```

For `node:path`:

```sh
npm run generate:node-contract-path
npm run check:node-contract-path
npm run test:node-contract-path
```

For `node:child_process`:

```sh
npm run generate:node-contract-child-process
npm run check:node-contract-child-process
npm run test:node-contract-child-process
```

For `node:perf_hooks`:

```sh
npm run generate:node-contract-perf-hooks
npm run check:node-contract-perf-hooks
npm run test:node-contract-perf-hooks
```

For `node:process`:

```sh
npm run generate:node-contract-process
npm run check:node-contract-process
npm run test:node-contract-process
```

For `node:buffer`, `node:events`, and `node:os`:

```sh
npm run generate:node-contract-buffer
npm run check:node-contract-buffer
npm run test:node-contract-buffer

npm run generate:node-contract-events
npm run check:node-contract-events
npm run test:node-contract-events

npm run generate:node-contract-os
npm run check:node-contract-os
npm run test:node-contract-os
```

For `node:stream`, `node:stream/promises`, `node:util`, and `node:util/types`:

```sh
npm run generate:node-contract-stream
npm run check:node-contract-stream
npm run test:node-contract-stream

npm run generate:node-contract-stream-promises
npm run check:node-contract-stream-promises
npm run test:node-contract-stream-promises

npm run generate:node-contract-util
npm run check:node-contract-util
npm run test:node-contract-util

npm run generate:node-contract-util-types
npm run check:node-contract-util-types
npm run test:node-contract-util-types
```

For `node:zlib`, `node:string_decoder`, `node:timers`, and
`node:timers/promises`:

```sh
npm run generate:node-contract-zlib
npm run check:node-contract-zlib
npm run test:node-contract-zlib

npm run generate:node-contract-string-decoder
npm run check:node-contract-string-decoder
npm run test:node-contract-string-decoder

npm run generate:node-contract-timers
npm run check:node-contract-timers
npm run test:node-contract-timers

npm run generate:node-contract-timers-promises
npm run check:node-contract-timers-promises
npm run test:node-contract-timers-promises
```

Regenerate or check every configured contract in one command:

```sh
npm run generate:node-contracts
npm run check:node-contracts
```

Module selection and generated path metadata live in the generator's
`contractDefinitions` manifest. Add ordinary top-level modules there rather
than duplicating command-line selection branches.

Use cited `normalizedMethods` entries only when the official JSON loses
signature metadata or a JavaScript variadic call form cannot be represented by
the ordinary overload expansion. This keeps complex modules in the shared
pipeline without adding module-specific rendering code.

The checked-in override files record which contract members each intrinsic
module currently implements and how each implementation is invoked. The
generated intrinsic adapters use compile-time-bound calls for those members
and throw `NotImplementedException` for the remaining surface. Legacy
intrinsic methods that accept an `object[]` receive a statically generated
argument-array bridge; no runtime method discovery is used.

Do not edit any `*.Generated.cs` contract or intrinsic adapter directly.

## Faster iteration

Download each official JSON document once under `artifacts/nodeContracts` and
pass it through `--input` while developing. Modules and submodules from the
same document should share one lock and local input, as `timers` and
`timers/promises` do.

Classify the JSON shape before editing the generator: root methods, sibling API
sections, constructor-only modules, nested namespace aliases, and malformed
records each have existing extraction patterns. Run only the new module's
generate/check/test commands while the extraction stabilizes; run the
aggregate generator, contract project, and build once after the shared
generator is settled.
