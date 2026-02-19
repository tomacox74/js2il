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

Text-only, returns UTF-8 string. Buffer is not implemented. Recognizes string encoding option 'utf8'/'utf-8'.

**Tests:**
- `Js2IL.Tests.Node.FS.ExecutionTests.FS_ReadWrite_Utf8` (`Js2IL.Tests/Node/FS/ExecutionTests.cs`)
- `Js2IL.Tests.Node.FS.GeneratorTests.FS_ReadWrite_Utf8` (`Js2IL.Tests/Node/FS/GeneratorTests.cs`)

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

Text-only, accepts stringifiable data. Writes UTF-8. Recognizes string encoding option 'utf8'/'utf-8'.

**Tests:**
- `Js2IL.Tests.Node.FS.ExecutionTests.FS_ReadWrite_Utf8` (`Js2IL.Tests/Node/FS/ExecutionTests.cs`)
- `Js2IL.Tests.Node.FS.GeneratorTests.FS_ReadWrite_Utf8` (`Js2IL.Tests/Node/FS/GeneratorTests.cs`)
