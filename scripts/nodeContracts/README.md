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

The checked-in override files record which contract members each intrinsic
module currently implements. The generated intrinsic adapters delegate those
members and throw `NotImplementedException` for the remaining surface.

Do not edit any `*.Generated.cs` contract or intrinsic adapter directly.
