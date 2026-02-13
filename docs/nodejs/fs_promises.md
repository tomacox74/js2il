# Module: fs/promises

[Back to Index](Index.md)

| Property | Value |
| --- | --- |
| Type | module |
| Status | partial |
| Node.js Version | 22.x LTS |
| Documentation | [Node.js Docs](https://nodejs.org/api/fs.html#fspromisesapi) |

## Implementation

- `JavaScriptRuntime/Node/FSPromises.cs`

## APIs

| API | Kind | Status | Docs |
| --- | ---- | ------ | ---- |
| access(path, mode) | function | supported | [docs](https://nodejs.org/api/fs.html#fspromisesaccesspath-mode) |
| readdir(path, { withFileTypes: true }) | function | supported | [docs](https://nodejs.org/api/fs.html#fspromisesreaddirpath-options) |
| mkdir(path, { recursive: true }) | function | supported | [docs](https://nodejs.org/api/fs.html#fspromisesmkdirpath-options) |
| copyFile(src, dest) | function | supported | [docs](https://nodejs.org/api/fs.html#fspromisescopyfilesrc-dest-mode) |

## API Details

### access(path, mode)

Resolves when path exists, rejects otherwise. Mode is accepted but not enforced.

### readdir(path, { withFileTypes: true })

Returns an array of DirEnt-like objects with properties: name and methods isDirectory(), isFile().

### mkdir(path, { recursive: true })

Creates directories recursively.

### copyFile(src, dest)

Copies a file from src to dest (overwrites if destination exists).
