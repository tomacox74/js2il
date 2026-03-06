# Module: fs

[Back to Index](Index.md)

| Property | Value |
| --- | --- |
| Type | module |
| Status | partial |
| Node.js Version | 22.x LTS |
| Documentation | [Node.js Docs](https://nodejs.org/api/fs.html) |

## Implementation

- `JavaScriptRuntime/Node/FS.cs`

## APIs

| API | Kind | Status | Docs |
| --- | ---- | ------ | ---- |
| existsSync(path) | function | supported | [docs](https://nodejs.org/api/fs.html#fsexistssyncpath) |
| readdirSync(path, { withFileTypes: true }) | function | supported | [docs](https://nodejs.org/api/fs.html#fsreaddirsyncpath-options) |
| readdirSync(path) | function | supported | [docs](https://nodejs.org/api/fs.html#fsreaddirsyncpath-options) |
| readFileSync(path[, options]) | function | supported | [docs](https://nodejs.org/api/fs.html#fsreadfilesyncpath-options) |
| rmSync(path[, options]) | function | supported | [docs](https://nodejs.org/api/fs.html#fsrmsyncpath-options) |
| statSync(path) | function | supported | [docs](https://nodejs.org/api/fs.html#fsstatsyncpath-options) |
| writeFileSync(path, data[, options]) | function | supported | [docs](https://nodejs.org/api/fs.html#fswritefilesyncfile-data-options) |
| promises | property | supported | [docs](https://nodejs.org/api/fs.html#fspromisesapi) |
| readFile(path[, options], callback) | function | supported | [docs](https://nodejs.org/api/fs.html#fsreadfilepath-options-callback) |
| writeFile(file, data[, options], callback) | function | supported | [docs](https://nodejs.org/api/fs.html#fswritefilefile-data-options-callback) |
| copyFile(src, dest[, mode], callback) | function | partial | [docs](https://nodejs.org/api/fs.html#fscopyfilesrc-dest-mode-callback) |
| readdir(path[, options], callback) | function | supported | [docs](https://nodejs.org/api/fs.html#fsreaddirpath-options-callback) |
| mkdir(path[, options], callback) | function | supported | [docs](https://nodejs.org/api/fs.html#fsmkdirpath-options-callback) |
| stat(path, callback) | function | supported | [docs](https://nodejs.org/api/fs.html#fsstatpath-options-callback) |
| rm(path[, options], callback) | function | supported | [docs](https://nodejs.org/api/fs.html#fsrmpath-options-callback) |
| access(path[, mode], callback) | function | partial | [docs](https://nodejs.org/api/fs.html#fsaccesspath-mode-callback) |
| realpath(path[, options], callback) | function | supported | [docs](https://nodejs.org/api/fs.html#fsrealpathpath-options-callback) |

## API Details

### existsSync(path)

Returns true for existing files or directories.

**Tests:**
- `Js2IL.Tests.Node.FS.ExecutionTests.FS_ExistsSync_File_And_Directory` (`Js2IL.Tests/Node/FS/ExecutionTests.cs`)
- `Js2IL.Tests.Node.FS.ExecutionTests.FS_ExistsSync_EmptyPath_ReturnsFalse` (`Js2IL.Tests/Node/FS/ExecutionTests.cs`)

### readdirSync(path, { withFileTypes: true })

Returns an array of DirEnt-like objects with properties: name (string) and methods isDirectory(), isFile().

**Tests:**
- `Js2IL.Tests.Node.FS.ExecutionTests.FS_ReaddirSync_WithFileTypes` (`Js2IL.Tests/Node/FS/ExecutionTests.cs`)
- `Js2IL.Tests.Node.FS.ExecutionTests.FS_ReaddirSync_NonExistent_ReturnsEmpty` (`Js2IL.Tests/Node/FS/ExecutionTests.cs`)

### readdirSync(path)

Returns an array of names (strings).

**Tests:**
- `Js2IL.Tests.Node.FS.ExecutionTests.FS_ReaddirSync_Basic_Names` (`Js2IL.Tests/Node/FS/ExecutionTests.cs`)
- `Js2IL.Tests.Node.FS.ExecutionTests.FS_ReaddirSync_NonExistent_ReturnsEmpty` (`Js2IL.Tests/Node/FS/ExecutionTests.cs`)

### readFileSync(path[, options])

Returns Buffer by default, or string when encoding option is 'utf8'/'utf-8'. Supports 'utf8' encoding option.

**Tests:**
- `Js2IL.Tests.Node.FS.ExecutionTests.FS_ReadWrite_Utf8` (`Js2IL.Tests/Node/FS/ExecutionTests.cs`)
- `Js2IL.Tests.Node.FS.ExecutionTests.FS_ReadWrite_Buffer` (`Js2IL.Tests/Node/FS/ExecutionTests.cs`)
- `Js2IL.Tests.Node.FS.GeneratorTests.FS_ReadWrite_Utf8` (`Js2IL.Tests/Node/FS/GeneratorTests.cs`)
- `Js2IL.Tests.Node.FS.GeneratorTests.FS_ReadWrite_Buffer` (`Js2IL.Tests/Node/FS/GeneratorTests.cs`)

### rmSync(path[, options])

Removes a file or directory (recursive). Supports options.force to ignore errors.

**Tests:**
- `Js2IL.Tests.Node.FS.ExecutionTests.FS_RmSync_Removes_File_And_Directory` (`Js2IL.Tests/Node/FS/ExecutionTests.cs`)

### statSync(path)

Returns a minimal Stats-like object supporting size (number). Directories report size 0.

**Tests:**
- `Js2IL.Tests.Node.FS.ExecutionTests.FS_StatSync_FileSize` (`Js2IL.Tests/Node/FS/ExecutionTests.cs`)
- `Js2IL.Tests.Node.FS.ExecutionTests.FS_StatSync_NonExistentPath_ReturnsZero` (`Js2IL.Tests/Node/FS/ExecutionTests.cs`)

### writeFileSync(path, data[, options])

Writes data to a file, creating or overwriting it. Supports Buffer, byte array, or string content. Supports 'utf8' encoding option.

**Tests:**
- `Js2IL.Tests.Node.FS.ExecutionTests.FS_ReadWrite_Utf8` (`Js2IL.Tests/Node/FS/ExecutionTests.cs`)
- `Js2IL.Tests.Node.FS.ExecutionTests.FS_ReadWrite_Buffer` (`Js2IL.Tests/Node/FS/ExecutionTests.cs`)
- `Js2IL.Tests.Node.FS.GeneratorTests.FS_ReadWrite_Utf8` (`Js2IL.Tests/Node/FS/GeneratorTests.cs`)
- `Js2IL.Tests.Node.FS.GeneratorTests.FS_ReadWrite_Buffer` (`Js2IL.Tests/Node/FS/GeneratorTests.cs`)

### promises

Exposes the fs.promises API surface (see module: fs/promises). Note: method calls work, but reading methods as delegate-valued properties may not reflect Node behavior yet.

**Tests:**
- `Js2IL.Tests.Node.FS.ExecutionTests.FS_Promises_Property` (`Js2IL.Tests/Node/FS/ExecutionTests.cs`)

### readFile(path[, options], callback)

Callback-style async readFile; returns Buffer by default, or string when encoding option is 'utf8'/'utf-8'.

**Tests:**
- `Js2IL.Tests.Node.FS.ExecutionTests.FS_ReadFile_Callback_Utf8` (`Js2IL.Tests/Node/FS/ExecutionTests.cs`)
- `Js2IL.Tests.Node.FS.ExecutionTests.FS_ReadFile_Callback_MissingFile_ENOENT` (`Js2IL.Tests/Node/FS/ExecutionTests.cs`)

### writeFile(file, data[, options], callback)

Callback-style async writeFile; supports Buffer, byte array, or string content. Supports 'utf8' encoding option.

**Tests:**
- `Js2IL.Tests.Node.FS.ExecutionTests.FS_WriteFile_Callback_Utf8` (`Js2IL.Tests/Node/FS/ExecutionTests.cs`)

### copyFile(src, dest[, mode], callback)

Callback-style async copyFile; mode is accepted but not enforced.

**Tests:**
- `Js2IL.Tests.Node.FS.ExecutionTests.FS_CopyFile_Callback` (`Js2IL.Tests/Node/FS/ExecutionTests.cs`)

### readdir(path[, options], callback)

Callback-style async readdir; supports options.withFileTypes.

**Tests:**
- `Js2IL.Tests.Node.FS.ExecutionTests.FS_Readdir_Callback_Names` (`Js2IL.Tests/Node/FS/ExecutionTests.cs`)

### mkdir(path[, options], callback)

Callback-style async mkdir; supports options.recursive.

**Tests:**
- `Js2IL.Tests.Node.FS.ExecutionTests.FS_Mkdir_Callback_Recursive` (`Js2IL.Tests/Node/FS/ExecutionTests.cs`)

### stat(path, callback)

Callback-style async stat; returns a minimal Stats-like object supporting size.

**Tests:**
- `Js2IL.Tests.Node.FS.ExecutionTests.FS_Stat_Callback_FileSize` (`Js2IL.Tests/Node/FS/ExecutionTests.cs`)

### rm(path[, options], callback)

Callback-style async rm; supports options.force to ignore errors.

**Tests:**
- `Js2IL.Tests.Node.FS.ExecutionTests.FS_Rm_Callback_Removes_File` (`Js2IL.Tests/Node/FS/ExecutionTests.cs`)

### access(path[, mode], callback)

Callback-style async access; mode is accepted but not enforced.

**Tests:**
- `Js2IL.Tests.Node.FS.ExecutionTests.FS_Access_Callback` (`Js2IL.Tests/Node/FS/ExecutionTests.cs`)

### realpath(path[, options], callback)

Callback-style async realpath; returns absolute path using Path.GetFullPath().

**Tests:**
- `Js2IL.Tests.Node.FS.ExecutionTests.FS_Realpath_Callback` (`Js2IL.Tests/Node/FS/ExecutionTests.cs`)
