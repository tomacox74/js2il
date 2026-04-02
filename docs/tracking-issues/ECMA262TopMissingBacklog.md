# ECMA-262 Top Missing Features Backlog

> **Last Updated**: 2026-03-29
> Purpose: capture the highest-impact ECMA-262 gaps still marked missing or materially incomplete in `docs\ECMA262`.
> Source basis: current `docs\ECMA262\**\Section*.json` tracking only; this pass intentionally avoids stale issue / PR history.

## How this pass was filtered

- Included items that are still marked `Not Yet Supported`, plus a few materially incomplete user-visible surfaces where the docs describe a large missing feature family.
- Collapsed closely related clauses into a single backlog item when the docs describe one missing surface (for example, iterator helpers).
- De-prioritized deprecated or low-value gaps (`with`, `debugger`) and metadata-only constructor/prototype parity where the underlying syntax/runtime behavior already works.

## Current ranked backlog (recommended order)

| Rank | Issue | Missing feature | Key tracked clauses | Why it is still top-10 |
|---:|---|---|---|---|
| 1 | [#859](https://github.com/tomacox74/js2il/issues/859) | Remaining modern class element limitations | [`15.7 Class Definitions`](../ECMA262/15/Section15_7.md) - `15.7.1`, `15.7.10`, `15.7.11` | Most modern class elements now work, but arbitrary runtime-computed method keys and the remaining class-element edge cases tracked in the docs are still incomplete. |

## Notable next-tier gaps

- **TypedArray constructor / prototype fidelity and remaining methods**: [`23.2 TypedArray Objects`](../ECMA262/23/Section23_2.md) and [`10.4.5 TypedArray Exotic Objects`](../ECMA262/10/Section10_4.md) are still `Incomplete`, but the docs now track a substantial supported subset for `Int32Array`, `Uint8Array`, and `Float64Array`.
- **Uint8Array base64 / hex extensions**: [`23.3 Uint8Array Objects`](../ECMA262/23/Section23_3.md) is still `Not Yet Supported`.
- **Promise follow-up surface**: [`27.2 Promise Objects`](../ECMA262/27/Section27_2.md) still has gaps such as `HostPromiseRejectionTracker`, `Promise.try`, and `Symbol.species`.
- **Function-constructor intrinsics for generator / async function families**: [`27.3`](../ECMA262/27/Section27_3.md), [`27.4`](../ECMA262/27/Section27_4.md), and [`27.7`](../ECMA262/27/Section27_7.md) still miss constructor / prototype parity even though the underlying syntax is mostly supported.

## Intentionally de-prioritized missing items

- [`14.11 The with Statement`](../ECMA262/14/Section14_11.md)
- [`14.16 The debugger Statement`](../ECMA262/14/Section14_16.md)

These are still marked unsupported, but they are low-value or intentionally rejected compared with the ten gaps above.
