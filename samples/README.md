# Samples

These samples demonstrate consuming **compiled** JavaScript modules as a .NET library through generated facades.

- [samples/Basic](Basic/README.md): import a compiled module through its generated facade.
- [samples/Typed](Typed/README.md): generated typed exports, class construction, and async calls.
- [samples/Domino](Domino/README.md): compile and host a real npm package (@mixmark-io/domino).
- [samples/Picocolors](Picocolors/README.md): compile and host the picocolors npm package (ANSI color helpers).
- [samples/NpmRunAll2](NpmRunAll2/README.md): compile npm-run-all2 and call its task-header and pattern-matching utilities from C#.

## How samples work

Most samples are split into two parts:

- `compiler/` – the JavaScript source inputs consumed during `dotnet build`.
- `host/` – a C# console app that restores `Jroc.SDK`, compiles the JavaScript
  input via `JrocCompile`, and calls the generated `Run`/`Import` facade. The
  SDK supplies the runtime implementation transitively.

`Domino`, `Picocolors`, and `NpmRunAll2` are the exceptions: they keep `package.json` / `package-lock.json` next to the `.csproj`, run `npm ci` in place, and compile the npm package directly by module id or file path with the SDK defaults. Package `types` / `typings` metadata can supply generated facade shapes while execution still uses the resolved JavaScript entrypoint.
