# Node module support punch list (internal scripts)

This checklist captures Node.js module APIs required so internal project scripts in scripts/ can be compiled with js2il.

## child_process
Required by:
- scripts/decompileGeneratorTest.js
- scripts/installLocalTool.js
- scripts/release.js
- scripts/runExecutionTestsAndReportFailures.js
- scripts/runGeneratorTestsAndUpdateFailures.js
- scripts/syncExecutionSnapshots.js

APIs:
- spawnSync(command, args, options)
- execSync(command, options)

Notes:
- scripts use both node:child_process and child_process module specifiers.
- spawnSync options used: cwd, stdio, shell, encoding.

## fs/promises
Required by:
- scripts/updateVerifiedFiles.js

APIs:
- access(path, mode)
- readdir(path, options) (withFileTypes: true)
- mkdir(path, options) (recursive: true)
- copyFile(src, dest)

## os
Required by:
- scripts/decompileGeneratorTest.js
- scripts/installLocalTool.js

APIs:
- tmpdir()
- homedir()

