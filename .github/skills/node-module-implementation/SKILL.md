---
name: node-module-implementation
description: Implement a new Node.js module or add public runtime support to an existing Node module, including the required generated Node contract and static intrinsic adapter update.
tier: standard
applyTo: 'src/JavaScriptRuntime/Node/**,tests/Jroc.Tests/Node/**,tests/Jroc.NodeContracts.Tests/**,scripts/nodeContracts/**,docs/nodejs/**,package.json,CHANGELOG.md'
---

# Node Module Implementation

Use this skill when adding a new `[NodeModule]` runtime implementation or
implementing a public member on an existing Node module.

## Completion Rule

A public Node runtime implementation is not complete until its generated Node
contract is present and current.

Always use the `node-module-contracts` skill as the authoritative contract
workflow. Do not merge a runtime-only implementation and defer its contract to
later work.

## Decide Which Contract Path Applies

### New Node module

1. Implement and register the intrinsic under
   `src/JavaScriptRuntime/Node`.
2. Generate the module's complete top-level public contract from the pinned
   official Node.js 24 LTS documentation.
3. Add the lock, narrow overrides, manifest entry, package scripts, generated
   interface, generated static intrinsic adapter, and manual contract tests
   required by the `node-module-contracts` skill.
4. Keep the contract complete even when the new runtime implements only part
   of the documented API. Unavailable members must remain in the contract and
   throw `NotImplementedException`.

### Public member on an existing Node module

1. Find the module's generated interface and override file before changing the
   runtime.
2. Confirm the official member already exists in the generated contract.
   - If it exists, add or update its `intrinsicImplementations` mapping.
   - If it is missing, update the pinned extraction or cited normalization so
     the complete official member is generated; do not hand-edit generated C#.
3. Make the runtime signature resolvable by a generated compile-time adapter.
   Use the supported direct or argument-array bridge metadata rather than
   reflection or late-bound invocation.
4. Regenerate the module interface and intrinsic adapter even when the public
   interface text itself does not change.
5. Add contract coverage proving direct delegation and runtime coverage proving
   Node-compatible behavior.

## Required Implementation Workflow

1. Read the existing runtime module, focused runtime tests, contract lock,
   overrides, generated interface, generated adapter, and manual contract
   tests.
2. Add failing focused runtime coverage under
   `tests/Jroc.Tests/Node/<Module>`.
3. Implement Node-compatible behavior and errors. Compatibility takes priority
   over performance.
4. Perform the applicable contract path above.
5. Update `docs/nodejs/<module>.json` to reflect newly implemented support and
   regenerate the Node support docs.
6. Update `CHANGELOG.md` for a new module or meaningful public runtime support.

## Contract Boundaries

- The official Node.js 24 LTS documentation defines the contract; the runtime
  implementation does not.
- Do not omit unimplemented APIs from a new contract.
- Do not expose concrete runtime `Array` or `Promise` types in the public ABI.
- Do not manually edit `*.Generated.cs`.
- Do not add reflection, generic dispatch, or success-shaped placeholders.
- Nested class and stable-shape APIs still require the shared nested-contract
  architecture tracked by #1660. Do not silently treat that limitation as a
  reason to skip contract follow-through.

## Focused Validation

Run the smallest checks that cover the changed module:

```sh
npm run generate:node-contract-<name>
npm run check:node-contract-<name>
npm run test:node-contract-<name>
dotnet test tests/Jroc.Tests/Jroc.Tests.csproj --nologo \
  --filter FullyQualifiedName~Jroc.Tests.Node.<Module>
dotnet build jroc.sln --no-restore --nologo
```

If the shared contract generator changes, regenerate and check every contract
once after the generator stabilizes.

Do not run the full Jroc or test262 suites locally. Push the focused,
build-clean change to a PR and use the parallel CI build, canary, and test262
results to guide any remaining fixes.

## Pull Request Checklist

- Runtime implementation and focused behavior tests are included.
- The complete generated contract exists.
- `intrinsicImplementations` statically binds every newly implemented public
  member.
- Generated interfaces and adapters are current.
- Manual contract tests cover representative delegation and unavailable
  fallbacks.
- Node support docs and changelog are current.
- Required PR CI checks are green.
