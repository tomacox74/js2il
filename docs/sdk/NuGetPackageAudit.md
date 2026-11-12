# NuGet package presentation audit

Audit for [#2468](https://github.com/tomacox74/js2il/issues/2468), as of
**2026-11-12 UTC**. Published evidence is the four NuGet.org **0.12.36**
listings and downloaded `.nupkg` files, all associated with release commit
`f86bce4c4d10ddee565ccfac9a997e4e049c2413`. Initial repository findings use
master `36627f9203699e3785c9c1a25f657c0501a50c2a` plus this audit's changes.
Integrated CLI guidance also preserves the later standalone target-framework
support from #2476 and usage documentation from #2482.

Repository/README changes appear on NuGet.org only after a new release.
Published `0.12.36` is not modified, and its SDK does not contain the
unreleased cross-major targeting fix from #2472.

## Per-package findings

All four already embed a package-specific `README.md` and the same
`icon.jpg`, use SPDX `Apache-2.0`, carry Git repository/commit metadata,
identify author `tomacox74`, and have relevant role-specific tags.

| Package | Published/current state | Actual gap | Change or recommendation | Priority |
|---|---|---|---|---|
| [`jroc`](https://www.nuget.org/packages/jroc/0.12.36) | `net10.0` global tool, major roll-forward, bundled dependencies and portable PDBs; no NuGet dependency group | No homepage/release notes; generic description; README mislabels `-a` as unused analysis and assumes a repository-only input file | Add quick-start homepage and release-history metadata, precise IL-compiler description, correct option table and complete hello example | High |
| [`Jroc.Core`](https://www.nuget.org/packages/Jroc.Core/0.12.36) | `lib/net10.0/Jroc.Compiler.dll`; runtime and compiler dependencies; SDK-index homepage | Broken `tomacox74/jroc` links; description obscures runtime-supplied workflow; direct `module.Exports.Invoke(...)` example fails C# compilation; no distributed PDB in package | Correct links/description, cast the `object`-typed exports proxy to `JsDynamicExports`, clarify host/generated runtime requirements, add release history; symbols follow-up #2478 | High |
| [`Jroc.Runtime`](https://www.nuget.org/packages/Jroc.Runtime/0.12.36) | `lib/net10.0/JavaScriptRuntime.dll`; Acornima dependency; Unicode license preserved | Broken repository links; no minimal executable example or explicit compatibility guidance; no PDB in package | Add working dependency/exports example, correct links, explain generated requirements and release history; symbols follow-up #2478 | High |
| [`Jroc.SDK`](https://www.nuget.org/packages/Jroc.SDK/0.12.36) | `build/` integration, `tasks/net10.0` compiler/task assets, bundled samples, transitive runtime | Broken repository links; examples omit the JS fixture; gallery framework display does not explain task-host versus consumer target; no PDB in package | Supply complete JS/project/C# example, document the validated SDK/target matrix, correct links and release history; symbols follow-up #2478 | High |

### Metadata decisions

Package IDs already distinguish the four workflows; renaming them would be
disruptive. Optional titles are unset and the IDs provide recognizable
display names. Do not add deprecated `summary`, `licenseUrl`, or `iconUrl`
project properties: use descriptions, SPDX licenses, and embedded icons.
The pack SDK generates a compatibility `licenseUrl` in the nuspec; that
does not require adding a deprecated property.

Existing tags, repository URL/type, authors, library/SDK project URLs, and
license are appropriate and remain unchanged. Authors metadata is not proof
of gallery ownership or verification. Copyright is unset; do not invent a
legal attribution/date range. New `PackageReleaseNotes` values explicitly link
to the existing release history, rather than copying potentially stale notes
or claiming changes absent from a published version.

## Icon and rendering

The four published package icons and NuGet gallery image responses are
byte-identical: JPEG, **315,191 bytes**, **1280 x 720**,
SHA-256 `083a860b1eee3b979318904117ddd09cb2fc70469617d6fd3b96f4d47a2cc6c3`.
Embedding and gallery delivery work. The landscape IL-text image is dense at
thumbnail size and contains a small third-party watermark; it is not a clean,
recognizable square JROC mark.

NuGet recommends a **128 x 128 PNG/JPEG**, at most **1 MB**. Retain the current
shared asset until a branding/provenance decision is made; #2479 tracks a
replacement and 32/64/128-pixel light/dark checks.

Published gallery READMEs render and correspond to their package contents.
Candidate READMEs use ordinary headings, tables, links, and fenced code; no
new externally hosted images are introduced. Local package-byte/link checks
and GitHub Markdown rendering do not substitute for NuGet's authenticated
upload preview or Visual Studio UI review. A working graphical browser and
Visual Studio were unavailable in this Linux environment. Those graphical
checks remain explicit release/branding follow-ups, not claimed successes.

## Source provenance and debugging

Every published nuspec has the correct Git URL and exact release commit.
The CLI's three bundled portable PDBs contain Source Link document mappings:

```text
/_/* -> https://raw.githubusercontent.com/tomacox74/js2il/f86bce4c4d10ddee565ccfac9a997e4e049c2413/*
```

Published Core/Runtime/SDK DLL debug directories contain `CodeView`,
`PdbChecksum`, and `Reproducible` entries, not embedded portable PDBs. Their
`.nupkg` files contain no PDBs. The repository release path does not enable
`IncludeSymbols` / `SymbolPackageFormat` or create `.snupkg` files. This audits
the shipped packages and release configuration, not every possible manual
symbol-server upload.

Evaluated .NET 10 SDK properties already include `Deterministic=true`,
`DebugType=portable`, `EnableSourceLink=true`, and
`EmbedUntrackedSources=true`. The publish workflow already packs with
`ContinuousIntegrationBuild=true`, and the SDK supplies GitHub Source Link
by default. No new Source Link dependency or redundant build-property layer
is warranted. Deterministic settings and PE markers are evidence, not a
two-machine byte-for-byte reproducibility certification.

The remaining gap is **symbol distribution**. #2478 tracks standard portable
`.snupkg` publication or an explicitly chosen embedded-PDB policy, identity
checks, and real debugger retrieval. Keep publication behind the existing
exact-SHA/trusted-publishing gate. Library/task symbols are distinct from
`EmitPdb` / `--pdb` symbols for newly generated JavaScript assemblies.

## .NET compatibility

No dependency version or package target framework changes are needed.
The following matrix describes the **working-tree candidate**, including
the previously merged but unreleased SDK targeting fix:

| Workflow | Compiler/task execution | Consumer/generated requirement | Evidence |
|---|---|---|---|
| CLI | .NET 10; packaged `net10.0` tool | Generated runtime follows .NET 10 execution | Packed hello example on .NET 10-only installation |
| CLI | .NET 11 via major roll-forward | Generated runtime follows .NET 11 execution, not `net10.0` package metadata | Same packed tool on .NET 11-only installation; .NET 10-only execution correctly rejected |
| Core + Runtime | .NET 10 or .NET 11 host | In-memory artifact follows the executing host runtime | Exact package README examples built/executed on each major |
| SDK | SDK 10 | `net10.0` target: generated `System.Runtime` 10, executes on .NET 10 | Packaged Basic matrix |
| SDK | SDK 11 | `net10.0` target: generated `System.Runtime` 10, executes on .NET 10-only installation | Packaged Basic matrix; no MSB3277/CS1705 |
| SDK | SDK 11 | `net11.0` target: generated `System.Runtime` 11, executes on .NET 11 | Packaged Basic matrix |

The CLI checks above describe default output without `--target-framework`.
Standalone compilation can now explicitly select `net10.0` or `net11.0`
using the matching reference pack; see
[Selecting the output framework](../NuGet.README.md#selecting-the-output-framework).

The SDK is a task package without `lib/` assets. NuGet's framework tab is
not a complete consumer-support matrix; do not add fake library assets to
alter that display. SDK 10 cannot target .NET 11. A newer compiler SDK does
not require retargeting an existing .NET 10 SDK consumer.
Framework compatibility is not a claim of complete ECMAScript/Node.js support.

## Legacy packages and verification

Owner-scoped NuGet search found exactly the four current packages and
`js2il`, `Js2IL.Core`, `Js2IL.Runtime`, `Js2IL.SDK`, whose latest visible
versions were **0.9.27**. Legacy gallery pages still recommend the old family;
no migration/deprecation banner was displayed.

The [migration guide](LegacyPackageMigration.md) maps all four IDs, tool
manifests, central versions, MSBuild items, and current consumer APIs.
Recompile JS; do not replace old runtime DLLs or promise source/binary
compatibility. #2480 tracks maintainer-approved gallery **Legacy** deprecation
with alternate-package metadata and a guide link. Deprecation is not unlisting;
no old package was altered, unpublished, or made unavailable.

NuGet search reported `verified=false` for all four JROC IDs, with the same
displayed owner. Prefix reservation is a discretionary server/account-level
approval, not a project property or Microsoft certification. `Jroc` is four
characters and identifies the project, but NuGet also considers ownership,
namespace confusion, and generic names.

#2477 tracks the owner's request to `account@nuget.org`: include NuGet display
name, ownership/project evidence, desired prefixes, and delegations; explicitly
confirm coverage of both bare lowercase `jroc` and `Jroc.*`.
Approval must precede any verified claim. Check the indicator in NuGet.org
and Visual Studio with nuget.org as the single selected feed. Existing matching
third-party packages are not automatically removed by a reservation.

## Validation and release follow-through

Local `0.12.36-audit` packages are isolated inspection candidates, not a
version bump or publication. Inspect all four nuspecs, dependency versions,
exact README/icon bytes, runtime license assets, and repository commit.

Focused coverage in `JrocSdkPackageTests`:

| Requirement | Evidence |
|---|---|
| Metadata/assets, homepage, release history, corrected links | `Pack_JrocTool_DoesNotShipHostingSamples`, `Pack_JrocCore_ContainsReadmeIconAndDiscoverabilityMetadata`, `Pack_JrocRuntime_ContainsReadmeIconAndDiscoverabilityMetadata`, `Pack_JrocSdk_ContainsBuildAssetsSamplesAndRuntimeDependency` |
| Actual packed C# README snippets compile and return the documented result | `Build_PackedInMemoryReadmeExample_CompilesAndRuns` (Core and Runtime cases) |
| Same candidate works across SDK 10/target 10, SDK 11/target 10, SDK 11/target 11 | `Build_ExtractedBasicSample_WithLocalJrocSdkPackage_CompilesAndRuns`, with both isolated SDK paths supplied |

The .NET 10/11 example checks use package references/local tool installation
in scratch consumers, not source project references. Retain test reports and
compare the packaged Basic generated references/runtime configuration with
the executing major-only runtime.

Before a future release, run the existing [coordinated release gate](PackagingValidation.md),
preview new package pages through NuGet's upload portal without publishing,
and check the Visual Studio presentation. After publication, confirm the
actual gallery metadata at the new version; repository edits alone do not
update `0.12.36`.

## Official guidance

- [NuGet metadata, icon, and deprecated fields](https://learn.microsoft.com/en-us/nuget/reference/nuspec)
- [Package READMEs and upload preview](https://learn.microsoft.com/en-us/nuget/nuget-org/package-readme-on-nuget-org)
- [SDK-integrated Source Link](https://github.com/dotnet/sourcelink#using-source-link-in-net-projects)
- [Portable symbol packages and embedded-symbol alternative](https://learn.microsoft.com/en-us/nuget/create-packages/symbol-packages-snupkg)
- [Legacy deprecation and alternative-package metadata](https://learn.microsoft.com/en-us/nuget/nuget-org/deprecate-packages)
- [Verified-prefix eligibility and application](https://learn.microsoft.com/en-us/nuget/nuget-org/id-prefix-reservation)
