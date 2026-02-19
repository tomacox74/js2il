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
| readFile(path, options) | function | supported | [docs](https://nodejs.org/api/fs.html#fspromisesreadfilepath-options) |
| writeFile(path, data, options) | function | supported | [docs](https://nodejs.org/api/fs.html#fspromiseswritefilefile-data-options) |
| stat(path) | function | supported | [docs](https://nodejs.org/api/fs.html#fspromisesstatpath-options) |
| lstat(path) | function | supported | [docs](https://nodejs.org/api/fs.html#fspromiseslstatpath-options) |
| realpath(path) | function | supported | [docs](https://nodejs.org/api/fs.html#fspromisesrealpathpath-options) |

## API Details

### access(path, mode)

Resolves when path exists, rejects otherwise. Mode is accepted but not enforced.

### readdir(path, { withFileTypes: true })

Returns an array of DirEnt-like objects with properties: name and methods isDirectory(), isFile().

### mkdir(path, { recursive: true })

Creates directories recursively.

### copyFile(src, dest)

Copies a file from src to dest (overwrites if destination exists).

### readFile(path, options)

Returns a Promise resolving to Buffer by default, or string when encoding option is 'utf8'. Supports 'utf8' encoding.

### writeFile(path, data, options)

Writes data to a file, creating or overwriting it. Supports Buffer, byte array, or string content. Supports 'utf8' encoding option.

### stat(path)

Returns a Promise resolving to a Stats object with a 'size' property. Rejects if path doesn't exist.

### lstat(path)

Currently behaves the same as stat() due to .NET limitations. Returns Stats object with 'size' property.

### realpath(path)

Returns a Promise resolving to the absolute path using Path.GetFullPath().
