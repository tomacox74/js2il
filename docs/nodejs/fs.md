# Module: fs

[Back to Index](Index.md)

| Property | Value |
| --- | --- |
| Type | module |
| Status | partial |
| Node.js Version | 22.x LTS |
| Documentation | [Node.js Docs](https://nodejs.org/api/fs.html) |

## Implementation

- `src/JavaScriptRuntime/Node/FS.cs`
- `src/JavaScriptRuntime/Node/FsFileHandle.cs`
- `src/JavaScriptRuntime/Node/FsCommon.cs`

## APIs

| API | Kind | Status | Docs |
| --- | ---- | ------ | ---- |
| existsSync(path) | function | supported | [docs](https://nodejs.org/api/fs.html#fsexistssyncpath) |
| readdirSync(path, { withFileTypes: true }) | function | supported | [docs](https://nodejs.org/api/fs.html#fsreaddirsyncpath-options) |
| readdirSync(path) | function | supported | [docs](https://nodejs.org/api/fs.html#fsreaddirsyncpath-options) |
| readFileSync(path[, options]) | function | supported | [docs](https://nodejs.org/api/fs.html#fsreadfilesyncpath-options) |
| rmSync(path[, options]) | function | supported | [docs](https://nodejs.org/api/fs.html#fsrmsyncpath-options) |
| unlinkSync(path) | function | supported | [docs](https://nodejs.org/api/fs.html#fsunlinksyncpath) |
| statSync(path) | function | supported | [docs](https://nodejs.org/api/fs.html#fsstatsyncpath-options) |
| writeFileSync(path, data[, options]) | function | supported | [docs](https://nodejs.org/api/fs.html#fswritefilesyncfile-data-options) |
| promises | property | supported | [docs](https://nodejs.org/api/fs.html#fspromisesapi) |
| open(path[, flags[, mode]], callback) | function | partial | [docs](https://nodejs.org/api/fs.html#fsopenpath-flags-mode-callback) |
| createReadStream(path[, options]) | function | supported | [docs](https://nodejs.org/api/fs.html#fscreatereadstreampath-options) |
| createWriteStream(path[, options]) | function | supported | [docs](https://nodejs.org/api/fs.html#fscreatewritestreampath-options) |
| readFile(path[, options], callback) | function | supported | [docs](https://nodejs.org/api/fs.html#fsreadfilepath-options-callback) |
| appendFile(file, data[, options], callback) | function | supported | [docs](https://nodejs.org/api/fs.html#fsappendfilepath-data-options-callback) |
| rename(oldPath, newPath, callback) | function | supported | [docs](https://nodejs.org/api/fs.html#fsrenameoldpath-newpath-callback) |
| unlink(path, callback) | function | supported | [docs](https://nodejs.org/api/fs.html#fsunlinkpath-callback) |
| writeFile(file, data[, options], callback) | function | supported | [docs](https://nodejs.org/api/fs.html#fswritefilefile-data-options-callback) |
| copyFile(src, dest[, mode], callback) | function | partial | [docs](https://nodejs.org/api/fs.html#fscopyfilesrc-dest-mode-callback) |
| readdir(path[, options], callback) | function | supported | [docs](https://nodejs.org/api/fs.html#fsreaddirpath-options-callback) |
| mkdir(path[, options], callback) | function | supported | [docs](https://nodejs.org/api/fs.html#fsmkdirpath-options-callback) |
| stat(path, callback) | function | supported | [docs](https://nodejs.org/api/fs.html#fsstatpath-options-callback) |
| watch(filename[, options][, listener]) | function | not-supported | [docs](https://nodejs.org/api/fs.html#fswatchfilename-options-listener) |
| rm(path[, options], callback) | function | supported | [docs](https://nodejs.org/api/fs.html#fsrmpath-options-callback) |
| access(path[, mode], callback) | function | partial | [docs](https://nodejs.org/api/fs.html#fsaccesspath-mode-callback) |
| realpath(path[, options], callback) | function | supported | [docs](https://nodejs.org/api/fs.html#fsrealpathpath-options-callback) |

## API Details

### existsSync(path)

Returns true for existing files or directories.

**Tests:**
- `Jroc.Tests.Node.FS.ExecutionTests.FS_ExistsSync_File_And_Directory` (`tests/Jroc.Tests/Node/FS/ExecutionTests.cs`)
- `Jroc.Tests.Node.FS.ExecutionTests.FS_ExistsSync_EmptyPath_ReturnsFalse` (`tests/Jroc.Tests/Node/FS/ExecutionTests.cs`)

### readdirSync(path, { withFileTypes: true })

Returns an array of DirEnt-like objects with properties: name (string) and methods isDirectory(), isFile().

**Tests:**
- `Jroc.Tests.Node.FS.ExecutionTests.FS_ReaddirSync_WithFileTypes` (`tests/Jroc.Tests/Node/FS/ExecutionTests.cs`)
- `Jroc.Tests.Node.FS.ExecutionTests.FS_ReaddirSync_NonExistent_ReturnsEmpty` (`tests/Jroc.Tests/Node/FS/ExecutionTests.cs`)

### readdirSync(path)

Returns an array of names (strings).

**Tests:**
- `Jroc.Tests.Node.FS.ExecutionTests.FS_ReaddirSync_Basic_Names` (`tests/Jroc.Tests/Node/FS/ExecutionTests.cs`)
- `Jroc.Tests.Node.FS.ExecutionTests.FS_ReaddirSync_NonExistent_ReturnsEmpty` (`tests/Jroc.Tests/Node/FS/ExecutionTests.cs`)

### readFileSync(path[, options])

Returns Buffer by default, or string when encoding option is 'utf8'/'utf-8'. Supports 'utf8' encoding option.

**Tests:**
- `Jroc.Tests.Node.FS.ExecutionTests.FS_ReadWrite_Utf8` (`tests/Jroc.Tests/Node/FS/ExecutionTests.cs`)
- `Jroc.Tests.Node.FS.ExecutionTests.FS_ReadWrite_Buffer` (`tests/Jroc.Tests/Node/FS/ExecutionTests.cs`)
- `Jroc.Tests.Node.FS.GeneratorTests.FS_ReadWrite_Utf8` (`tests/Jroc.Tests/Node/FS/GeneratorTests.cs`)
- `Jroc.Tests.Node.FS.GeneratorTests.FS_ReadWrite_Buffer` (`tests/Jroc.Tests/Node/FS/GeneratorTests.cs`)

### rmSync(path[, options])

Removes a file or directory (recursive). Supports options.force to ignore errors.

**Tests:**
- `Jroc.Tests.Node.FS.ExecutionTests.FS_RmSync_Removes_File_And_Directory` (`tests/Jroc.Tests/Node/FS/ExecutionTests.cs`)

### unlinkSync(path)

Removes a file synchronously. Missing files throw ENOENT-style errors, and directory paths reject with EISDIR-style errors.

**Tests:**
- `Jroc.Tests.Node.FS.ExecutionTests.FS_UnlinkSync_ReleaseCleanup` (`tests/Jroc.Tests/Node/FS/ExecutionTests.cs`)
- `Jroc.Tests.Node.FS.GeneratorTests.FS_UnlinkSync_ReleaseCleanup` (`tests/Jroc.Tests/Node/FS/GeneratorTests.cs`)

### statSync(path)

Returns a practical Stats-like object with size, mode, atime/mtime/ctime/birthtime plus their `*Ms` number variants, and the common `isFile()` / `isDirectory()` / `isSymbolicLink()` / `isBlockDevice()` / `isCharacterDevice()` / `isFIFO()` / `isSocket()` predicates. `mode` and permission bits are best-effort platform-derived values, and `ctime` / `birthtime` currently both map to platform creation metadata. Non-existent paths still return the legacy zeroed fallback instead of throwing.

**Tests:**
- `Jroc.Tests.Node.FS.ExecutionTests.FS_StatSync_FileSize` (`tests/Jroc.Tests/Node/FS/ExecutionTests.cs`)
- `Jroc.Tests.Node.FS.ExecutionTests.FS_StatSync_RichMetadata` (`tests/Jroc.Tests/Node/FS/ExecutionTests.cs`)
- `Jroc.Tests.Node.FS.ExecutionTests.FS_StatSync_NonExistentPath_ReturnsZero` (`tests/Jroc.Tests/Node/FS/ExecutionTests.cs`)
- `Jroc.Tests.Node.FS.GeneratorTests.FS_StatSync_RichMetadata` (`tests/Jroc.Tests/Node/FS/GeneratorTests.cs`)

### writeFileSync(path, data[, options])

Writes data to a file, creating or overwriting it. Supports Buffer, byte array, or string content. Supports 'utf8' encoding option.

**Tests:**
- `Jroc.Tests.Node.FS.ExecutionTests.FS_ReadWrite_Utf8` (`tests/Jroc.Tests/Node/FS/ExecutionTests.cs`)
- `Jroc.Tests.Node.FS.ExecutionTests.FS_ReadWrite_Buffer` (`tests/Jroc.Tests/Node/FS/ExecutionTests.cs`)
- `Jroc.Tests.Node.FS.GeneratorTests.FS_ReadWrite_Utf8` (`tests/Jroc.Tests/Node/FS/GeneratorTests.cs`)
- `Jroc.Tests.Node.FS.GeneratorTests.FS_ReadWrite_Buffer` (`tests/Jroc.Tests/Node/FS/GeneratorTests.cs`)

### promises

Exposes the fs.promises API surface (see module: fs/promises). Note: method calls work, but reading methods as delegate-valued properties may not reflect Node behavior yet.

**Tests:**
- `Jroc.Tests.Node.FS.ExecutionTests.FS_Promises_Property` (`tests/Jroc.Tests/Node/FS/ExecutionTests.cs`)

### open(path[, flags[, mode]], callback)

Opens a file and passes back a FileHandle-like object exposing fd/read/write/close for the supported baseline. Supported flags: r, r+, w, w+, a, a+. The callback still receives the FileHandle object instead of Node's raw numeric fd, so raw fd-centric callback workflows remain unsupported.

**Tests:**
- `Jroc.Tests.Node.FS.ExecutionTests.FS_Open_Callback_FileHandle` (`tests/Jroc.Tests/Node/FS/ExecutionTests.cs`)
- `Jroc.Tests.Node.FS.GeneratorTests.FS_Open_Callback_FileHandle` (`tests/Jroc.Tests/Node/FS/GeneratorTests.cs`)

### createReadStream(path[, options])

Creates a Readable-backed file stream supporting utf8 encoding, highWaterMark chunking, and start/end byte ranges in the current baseline. Common failure paths surface through error/close events.

**Tests:**
- `Jroc.Tests.Node.FS.ExecutionTests.FS_CreateReadStream_Basic` (`tests/Jroc.Tests/Node/FS/ExecutionTests.cs`)
- `Jroc.Tests.Node.FS.ExecutionTests.FS_CreateReadStream_Missing_Error` (`tests/Jroc.Tests/Node/FS/ExecutionTests.cs`)
- `Jroc.Tests.Node.FS.GeneratorTests.FS_CreateReadStream_Basic` (`tests/Jroc.Tests/Node/FS/GeneratorTests.cs`)
- `Jroc.Tests.Node.FS.GeneratorTests.FS_CreateReadStream_Missing_Error` (`tests/Jroc.Tests/Node/FS/GeneratorTests.cs`)

### createWriteStream(path[, options])

Creates a Writable-backed file stream supporting string/Buffer writes, utf8 encoding, and w/w+/a/a+ style flags in the current baseline. Common failure paths surface through error/close events.

**Tests:**
- `Jroc.Tests.Node.FS.ExecutionTests.FS_CreateWriteStream_Basic` (`tests/Jroc.Tests/Node/FS/ExecutionTests.cs`)
- `Jroc.Tests.Node.FS.ExecutionTests.FS_CreateWriteStream_Missing_Error` (`tests/Jroc.Tests/Node/FS/ExecutionTests.cs`)
- `Jroc.Tests.Node.FS.GeneratorTests.FS_CreateWriteStream_Basic` (`tests/Jroc.Tests/Node/FS/GeneratorTests.cs`)
- `Jroc.Tests.Node.FS.GeneratorTests.FS_CreateWriteStream_Missing_Error` (`tests/Jroc.Tests/Node/FS/GeneratorTests.cs`)

### readFile(path[, options], callback)

Callback-style async readFile; returns Buffer by default, or string when encoding option is 'utf8'/'utf-8'.

**Tests:**
- `Jroc.Tests.Node.FS.ExecutionTests.FS_ReadFile_Callback_Utf8` (`tests/Jroc.Tests/Node/FS/ExecutionTests.cs`)
- `Jroc.Tests.Node.FS.ExecutionTests.FS_ReadFile_Callback_MissingFile_ENOENT` (`tests/Jroc.Tests/Node/FS/ExecutionTests.cs`)

### appendFile(file, data[, options], callback)

Callback-style async appendFile; supports Buffer, byte array, or string content. Supports the same utf8 encoding baseline as writeFile.

**Tests:**
- `Jroc.Tests.Node.FS.ExecutionTests.FS_Append_Rename_Unlink_Callback` (`tests/Jroc.Tests/Node/FS/ExecutionTests.cs`)
- `Jroc.Tests.Node.FS.GeneratorTests.FS_Append_Rename_Unlink_Callback` (`tests/Jroc.Tests/Node/FS/GeneratorTests.cs`)

### rename(oldPath, newPath, callback)

Callback-style async rename for file and directory paths with Node-like ENOENT/EACCES/EIO style errors in the supported baseline.

**Tests:**
- `Jroc.Tests.Node.FS.ExecutionTests.FS_Append_Rename_Unlink_Callback` (`tests/Jroc.Tests/Node/FS/ExecutionTests.cs`)
- `Jroc.Tests.Node.FS.GeneratorTests.FS_Append_Rename_Unlink_Callback` (`tests/Jroc.Tests/Node/FS/GeneratorTests.cs`)

### unlink(path, callback)

Callback-style async unlink for files. Directories reject with EISDIR-style errors in the supported baseline.

**Tests:**
- `Jroc.Tests.Node.FS.ExecutionTests.FS_Append_Rename_Unlink_Callback` (`tests/Jroc.Tests/Node/FS/ExecutionTests.cs`)
- `Jroc.Tests.Node.FS.GeneratorTests.FS_Append_Rename_Unlink_Callback` (`tests/Jroc.Tests/Node/FS/GeneratorTests.cs`)

### writeFile(file, data[, options], callback)

Callback-style async writeFile; supports Buffer, byte array, or string content. Supports 'utf8' encoding option.

**Tests:**
- `Jroc.Tests.Node.FS.ExecutionTests.FS_WriteFile_Callback_Utf8` (`tests/Jroc.Tests/Node/FS/ExecutionTests.cs`)

### copyFile(src, dest[, mode], callback)

Callback-style async copyFile; mode is accepted but not enforced.

**Tests:**
- `Jroc.Tests.Node.FS.ExecutionTests.FS_CopyFile_Callback` (`tests/Jroc.Tests/Node/FS/ExecutionTests.cs`)

### readdir(path[, options], callback)

Callback-style async readdir; supports options.withFileTypes.

**Tests:**
- `Jroc.Tests.Node.FS.ExecutionTests.FS_Readdir_Callback_Names` (`tests/Jroc.Tests/Node/FS/ExecutionTests.cs`)

### mkdir(path[, options], callback)

Callback-style async mkdir; supports options.recursive.

**Tests:**
- `Jroc.Tests.Node.FS.ExecutionTests.FS_Mkdir_Callback_Recursive` (`tests/Jroc.Tests/Node/FS/ExecutionTests.cs`)

### stat(path, callback)

Callback-style async stat; returns the same richer Stats-like surface as statSync(), including mode, timestamps, and common type predicates. `mode` and permission bits are best-effort platform-derived values, and `ctime` / `birthtime` currently both map to platform creation metadata.

**Tests:**
- `Jroc.Tests.Node.FS.ExecutionTests.FS_Stat_Callback_FileSize` (`tests/Jroc.Tests/Node/FS/ExecutionTests.cs`)
- `Jroc.Tests.Node.FS.ExecutionTests.FS_Stat_Callback_RichMetadata` (`tests/Jroc.Tests/Node/FS/ExecutionTests.cs`)

### watch(filename[, options][, listener])

File watching is not implemented yet in the current runtime, so watch-driven dev-server and incremental-build loops still require a host fallback.

### rm(path[, options], callback)

Callback-style async rm; supports options.force to ignore errors.

**Tests:**
- `Jroc.Tests.Node.FS.ExecutionTests.FS_Rm_Callback_Removes_File` (`tests/Jroc.Tests/Node/FS/ExecutionTests.cs`)

### access(path[, mode], callback)

Callback-style async access; mode is accepted but not enforced.

**Tests:**
- `Jroc.Tests.Node.FS.ExecutionTests.FS_Access_Callback` (`tests/Jroc.Tests/Node/FS/ExecutionTests.cs`)

### realpath(path[, options], callback)

Callback-style async realpath; returns absolute path using Path.GetFullPath().

**Tests:**
- `Jroc.Tests.Node.FS.ExecutionTests.FS_Realpath_Callback` (`tests/Jroc.Tests/Node/FS/ExecutionTests.cs`)
