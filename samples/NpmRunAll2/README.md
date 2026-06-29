# NpmRunAll2

This sample demonstrates compiling the [`npm-run-all2`](https://www.npmjs.com/package/npm-run-all2) npm package into a .NET assembly via `Jroc.SDK` and calling its utilities from C#.

`npm-run-all2` is a CLI tool for running multiple npm scripts in parallel or sequentially.  This sample exposes two of its utilities:

- **Task header formatting** (`npm-run-all2/lib/create-header`) – formats the `"> task-name"` header that npm-run-all2 prints before each task executes.
- **Pattern-based task selection** – filters a list of script names using the same glob rules as the npm-run-all2 CLI (`test:*` matches `test:unit`, `test:integration`, etc.).

## Prerequisites

- .NET SDK (see repo root `global.json`)
- Node.js + npm

## Structure

| File | Purpose |
|---|---|
| `package.json` / `package-lock.json` | npm manifest — pins `npm-run-all2@^8.0.0` |
| `index.js` | Wrapper that re-exports `createHeader` from npm-run-all2 and provides `filterTasks` |
| `NpmRunAll2.csproj` | C# host; compiles `index.js` via `Jroc.SDK` and uses `Jroc.Runtime` |
| `Program.cs` | Loads the compiled module and exercises both utilities |

## Running the sample

From `samples/NpmRunAll2`:

```
dotnet run -c Release
```

This will:

1. Run `npm ci` to restore `npm-run-all2` into `node_modules`
2. Compile `index.js` (and its `npm-run-all2` dependency) to `index.dll` via `Jroc.SDK`
3. Load the compiled module and call `taskHeader` and `filterTasks`

## Expected output

```
=== task headers ===
> build
> test:unit --reporter spec
> lint

=== pattern matching ===
  test:*             => [test:unit,test:integration,test:e2e]
  lint               => [lint]
  build              => [build]
  test:e2e           => [test:e2e]
done
```

## Why `index.js`?

`npm-run-all2`'s main entry point requires modules that spawn child processes and use dynamic `import()`.  `index.js` is a minimal wrapper that pulls in only the purely functional sub-modules (`create-header`), keeping the compiled assembly lightweight.
