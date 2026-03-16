# ECMA-262 Top Missing Features Backlog

> **Last Updated**: 2026-03-16
> Purpose: capture the highest-impact ECMA-262 gaps still marked missing or materially incomplete in `docs\ECMA262`.
> Source basis: current `docs\ECMA262\**\Section*.json` tracking only; this pass intentionally avoids stale issue / PR history.

## How this pass was filtered

- Included items that are still marked `Not Yet Supported`, plus a few materially incomplete user-visible surfaces where the docs describe a large missing feature family.
- Collapsed closely related clauses into a single backlog item when the docs describe one missing surface (for example, iterator helpers).
- De-prioritized deprecated or low-value gaps (`with`, `debugger`) and metadata-only constructor/prototype parity where the underlying syntax/runtime behavior already works.

## Current ranked backlog (recommended order)

| Rank | Issue | Missing feature | Key tracked clauses | Why it is still top-10 |
|---:|---|---|---|---|
| 1 | [#857](https://github.com/tomacox74/js2il/issues/857) | Full ECMA module record / linking / evaluation model | [`16.2 Modules`](../ECMA262/16/Section16_2.md) - `16.2.1` | Static `import` / `export` lowering exists, but the docs still mark the real module-record/link/evaluate model as `Not Yet Supported`; this is the largest remaining ESM compatibility gap. |
| 2 | [#859](https://github.com/tomacox74/js2il/issues/859) | Remaining modern class element forms | [`15.7 Class Definitions`](../ECMA262/15/Section15_7.md) - `15.7.1`, `15.7.10`, `15.7.11` | Computed method names, computed field names, private methods/accessors, and static blocks are all still marked `Not Yet Supported`. |
| 3 | [#861](https://github.com/tomacox74/js2il/issues/861) | Keyed collections as first-class JS constructor / prototype surfaces | [`24.1 Map`](../ECMA262/24/Section24_1.md) - `24.1.2.2`; [`24.2 Set`](../ECMA262/24/Section24_2.md) - `24.2.3.1`; [`24.3 WeakMap`](../ECMA262/24/Section24_3.md) - `24.3.2.1`; [`24.4 WeakSet`](../ECMA262/24/Section24_4.md) - `24.4.2.1` | The runtime can construct these collections in narrow cases, but the docs still mark the JS-visible constructor values / `.prototype` surfaces as missing, which breaks reflective and prototype-based code. |
| 4 | [#862](https://github.com/tomacox74/js2il/issues/862) | Map/Set iterable construction and prototype completion | [`24.1 Map`](../ECMA262/24/Section24_1.md) - `24.1.1.1`, `24.1.3.5`, `24.1.3.14`; [`24.2 Set`](../ECMA262/24/Section24_2.md) - `24.2.2.1`, `24.2.4.2`, `24.2.4.4`, `24.2.4.5` | `new Map(iterable)`, `new Set(iterable)`, `Map.prototype.forEach`, `Map.prototype[@@iterator]`, the missing core Set members, and the ES2025 Set algebra methods are all still not supported. |
| 5 | [#863](https://github.com/tomacox74/js2il/issues/863) | Advanced proxy traps and revocation | [`10.5 Proxy internal methods`](../ECMA262/10/Section10_5.md) - `10.5.10`; [`28.2 Proxy Objects`](../ECMA262/28/Section28_2.md) - `28.2.2.1` | JS2IL currently supports the get/set/has happy path, but delete/ownKeys/apply/construct/prototype traps and `Proxy.revocable` are still missing. |
| 6 | [#864](https://github.com/tomacox74/js2il/issues/864) | String prototype object, string iterator, and missing modern string APIs | [`22.1 String Objects`](../ECMA262/22/Section22_1.md) - `22.1.2.3`, `22.1.3.1`, `22.1.3.14`, `22.1.3.20`, `22.1.3.36`, `22.1.5` | The docs still mark the real `String.prototype` object, string iteration, and many common APIs (`fromCodePoint`, `raw`, `at`, `codePointAt`, `matchAll`, `padStart` / `padEnd`, `replaceAll`, well-formed Unicode helpers) as missing. |
| 7 | [#865](https://github.com/tomacox74/js2il/issues/865) | Iterator helpers plus the `Iterator` / `AsyncIterator` constructor surfaces | [`27.1 Iteration`](../ECMA262/27/Section27_1.md) - `27.1.2`, `27.1.3`, `27.1.4` | `for..of` and `for await..of` work, but the new iterator-helper family and the public `Iterator` / `AsyncIterator` objects are still entirely marked `Not Yet Supported`. |
| 8 | [#866](https://github.com/tomacox74/js2il/issues/866) | Arguments exotic objects / mapped `arguments` semantics | [`10.4 Built-in Exotic Objects`](../ECMA262/10/Section10_4.md) - `10.4.4` | Functions currently materialize `arguments` as plain arrays, so mapped/unmapped arguments objects and their aliasing semantics are still missing. |

## Notable next-tier gaps

- **TypedArray constructor / prototype fidelity and remaining methods**: [`23.2 TypedArray Objects`](../ECMA262/23/Section23_2.md) and [`10.4.5 TypedArray Exotic Objects`](../ECMA262/10/Section10_4.md) are still `Incomplete`, but the docs now track a substantial supported subset for `Int32Array`, `Uint8Array`, and `Float64Array`.
- **Uint8Array base64 / hex extensions**: [`23.3 Uint8Array Objects`](../ECMA262/23/Section23_3.md) is still `Not Yet Supported`.
- **Promise follow-up surface**: [`27.2 Promise Objects`](../ECMA262/27/Section27_2.md) still has gaps such as `HostPromiseRejectionTracker`, `Promise.try`, and `Symbol.species`.
- **Function-constructor intrinsics for generator / async function families**: [`27.3`](../ECMA262/27/Section27_3.md), [`27.4`](../ECMA262/27/Section27_4.md), and [`27.7`](../ECMA262/27/Section27_7.md) still miss constructor / prototype parity even though the underlying syntax is mostly supported.

## Intentionally de-prioritized missing items

- [`14.11 The with Statement`](../ECMA262/14/Section14_11.md)
- [`14.16 The debugger Statement`](../ECMA262/14/Section14_16.md)

These are still marked unsupported, but they are low-value or intentionally rejected compared with the ten gaps above.
