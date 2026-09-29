# Mitata benchmarks

This folder contains focused JavaScript benchmarks that use [mitata](https://github.com/evanwashere/mitata), following the structure of Bun's `bench` folder.

## Setup

```powershell
npm install
```

## Run

```powershell
npm run bench:string-width
```

To compile and run the benchmark through jroc:

```powershell
npm run bench:string-width:jroc
```

Set `JROC` to use a specific jroc executable or `Jroc.dll`; otherwise the script runs the repo's `src\Cli` project.

## Managed runtime comparisons

The published-release workflow runs both `runtime-baseline` and `string-width`
on the same runner. `runtime-baseline` compares Node, JROC, ClearScript, Jint,
YantraJS, and Okojo; `string-width` compares Node, JROC, ClearScript, Jint,
and Okojo (YantraJS is excluded because it currently crashes on this benchmark).
Each suite has its own results file, artifact, and Supabase ingestion step.
Manual dispatch still runs the selected benchmark only.

Managed runtimes execute an import-free bundle of the same benchmark body,
generated with esbuild so their hosts do not need Node.js module APIs.

```powershell
npm run bench:release
npm run bench:string-width:jint
```

Use repeated `--runtime` arguments to select a subset:

```powershell
node scripts/run-release-suite.mjs --runtime jroc --runtime jint --runtime yantrajs
```

## Backfill string-width for a published JROC release

From the repository root, with the GitHub CLI installed and authenticated, run:

```powershell
node scripts/dispatchStringWidthBenchmarks.js 0.12.29 --dry-run
node scripts/dispatchStringWidthBenchmarks.js 0.12.29
gh run list --workflow mitata-suite.yml --event workflow_dispatch --limit 5
```

Pass the desired published JROC package version explicitly (with or without a leading
`v`). The helper dispatches the workflow from `master` with `string-width`,
Node, JROC, ClearScript, Jint, and Okojo. It sends `jroc_package_version` so the
workflow installs that release rather than compiling the checkout. Use `--repo
owner/name` when running outside this repository, or `--ref` to select another
branch containing the workflow. Check the new run's benchmark and Supabase ingestion
steps before relying on the dashboard.
