"use strict";

const fs = require('fs');
const path = require('path');
const os = require('os');

const uniqueSuffix = `${Date.now()}-${Math.floor(Math.random() * 1000000000)}`;
const file = path.join(os.tmpdir(), `js2il-fs-stat-sync-${uniqueSuffix}.txt`);
const dir = path.join(os.tmpdir(), `js2il-fs-stat-sync-dir-${uniqueSuffix}`);

fs.rmSync(file, { force: true });
fs.rmSync(dir, { force: true, recursive: true });
fs.writeFileSync(file, 'hello', 'utf8');
fs.mkdirSync(dir, { recursive: true });

try {
    const fileStats = fs.statSync(file);
    const dirStats = fs.statSync(dir);

    console.log('FileSize:', fileStats.size);
    console.log('FileModeHasRegularBit:', (fileStats.mode & 32768) === 32768);
    console.log('FileIsFile:', fileStats.isFile());
    console.log('FileIsDirectory:', fileStats.isDirectory());
    console.log('FileHasMtimeMs:', typeof fileStats.mtimeMs === 'number' && fileStats.mtimeMs > 0);
    console.log('FileMtimeIsoLength:', fileStats.mtime.toISOString().length);
    console.log('FileBirthtimeMatchesType:', typeof fileStats.birthtimeMs === 'number');
    console.log('FileIsSymbolicLink:', fileStats.isSymbolicLink());

    console.log('DirModeHasDirectoryBit:', (dirStats.mode & 16384) === 16384);
    console.log('DirIsDirectory:', dirStats.isDirectory());
    console.log('DirIsFile:', dirStats.isFile());
} finally {
    fs.rmSync(file, { force: true });
    fs.rmSync(dir, { force: true, recursive: true });
}
