# Module: fs/promises

[Back to Index](Index.md)

| Property | Value |
| --- | --- |
| Type | module |
| Status | partial |
| Node.js Version | 22.x LTS |
| Documentation | [Node.js Docs](https://nodejs.org/api/fs.html#fspromisesapi) |

## Implementation

- `src/JavaScriptRuntime/Node/FSPromises.cs`
- `src/JavaScriptRuntime/Node/FsFileHandle.cs`
- `src/JavaScriptRuntime/Node/FsCommon.cs`

## Notes

The current fs/promises baseline covers whole-file helpers plus FileHandle open/read/write/close, createReadStream/createWriteStream support exposed from node:fs, and practical mutation helpers. Supported FileHandle flags are r, r+, w, w+, a, and a+.

## APIs

| API | Kind | Status | Docs |
| --- | ---- | ------ | ---- |
| access(path, mode) | function | supported | [docs](https://nodejs.org/api/fs.html#fspromisesaccesspath-mode) |
| readdir(path, { withFileTypes: true }) | function | supported | [docs](https://nodejs.org/api/fs.html#fspromisesreaddirpath-options) |
| mkdir(path, { recursive: true }) | function | supported | [docs](https://nodejs.org/api/fs.html#fspromisesmkdirpath-options) |
| copyFile(src, dest) | function | supported | [docs](https://nodejs.org/api/fs.html#fspromisescopyfilesrc-dest-mode) |
| open(path[, flags[, mode]]) | function | supported | [docs](https://nodejs.org/api/fs.html#fspromisesopenpath-flags-mode) |
| readFile(path, options) | function | supported | [docs](https://nodejs.org/api/fs.html#fspromisesreadfilepath-options) |
| writeFile(path, data, options) | function | supported | [docs](https://nodejs.org/api/fs.html#fspromiseswritefilefile-data-options) |
| appendFile(path, data[, options]) | function | supported | [docs](https://nodejs.org/api/fs.html#fspromisesappendfilepath-data-options) |
| rename(oldPath, newPath) | function | supported | [docs](https://nodejs.org/api/fs.html#fspromisesrenameoldpath-newpath) |
| unlink(path) | function | supported | [docs](https://nodejs.org/api/fs.html#fspromisesunlinkpath) |
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

### open(path[, flags[, mode]])

Returns a Promise resolving to a FileHandle supporting fd/read/write/close in the current baseline. Supported flags: r, r+, w, w+, a, and a+.

**Tests:**
- `Js2IL.Tests.Node.FS.ExecutionTests.FSPromises_Open_Read_Write_Close` (`Js2IL.Tests/Node/FS/ExecutionTests.cs`)
- `Js2IL.Tests.Node.FS.GeneratorTests.FSPromises_Open_Read_Write_Close` (`Js2IL.Tests/Node/FS/GeneratorTests.cs`)

### readFile(path, options)

Returns a Promise resolving to Buffer by default, or string when encoding option is 'utf8'. Supports 'utf8' encoding.

### writeFile(path, data, options)

Writes data to a file, creating or overwriting it. Supports Buffer, byte array, or string content. Supports 'utf8' encoding option.

### appendFile(path, data[, options])

Appends Buffer, byte array, or string content to a file using the same utf8 encoding baseline as writeFile.

**Tests:**
- `Js2IL.Tests.Node.FS.ExecutionTests.FSPromises_Append_Rename_Unlink` (`Js2IL.Tests/Node/FS/ExecutionTests.cs`)
- `Js2IL.Tests.Node.FS.GeneratorTests.FSPromises_Append_Rename_Unlink` (`Js2IL.Tests/Node/FS/GeneratorTests.cs`)

### rename(oldPath, newPath)

Renames files or directories with Node-like ENOENT/EACCES/EIO style errors in the supported baseline.

**Tests:**
- `Js2IL.Tests.Node.FS.ExecutionTests.FSPromises_Append_Rename_Unlink` (`Js2IL.Tests/Node/FS/ExecutionTests.cs`)
- `Js2IL.Tests.Node.FS.GeneratorTests.FSPromises_Append_Rename_Unlink` (`Js2IL.Tests/Node/FS/GeneratorTests.cs`)

### unlink(path)

Removes files with Node-like ENOENT/EISDIR/EACCES error reporting in the supported baseline.

**Tests:**
- `Js2IL.Tests.Node.FS.ExecutionTests.FSPromises_Append_Rename_Unlink` (`Js2IL.Tests/Node/FS/ExecutionTests.cs`)
- `Js2IL.Tests.Node.FS.GeneratorTests.FSPromises_Append_Rename_Unlink` (`Js2IL.Tests/Node/FS/GeneratorTests.cs`)

### stat(path)

Returns a Promise resolving to a Stats object with a 'size' property. Rejects if path doesn't exist.

### lstat(path)

Currently behaves the same as stat() due to .NET limitations. Returns Stats object with 'size' property.

### realpath(path)

Returns a Promise resolving to the absolute path using Path.GetFullPath().
