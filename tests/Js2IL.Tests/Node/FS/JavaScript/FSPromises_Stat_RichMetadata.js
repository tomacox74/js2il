"use strict";

const fs = require('fs/promises');
const syncFs = require('fs');
const path = require('path');
const os = require('os');

const uniqueSuffix = `${Date.now()}-${Math.floor(Math.random() * 1000000000)}`;
const file = path.join(os.tmpdir(), `js2il-fs-promises-stat-${uniqueSuffix}.txt`);
const dir = path.join(os.tmpdir(), `js2il-fs-promises-stat-dir-${uniqueSuffix}`);

syncFs.rmSync(file, { force: true });
syncFs.rmSync(dir, { force: true, recursive: true });
syncFs.writeFileSync(file, 'hello', 'utf8');
syncFs.mkdirSync(dir, { recursive: true });

let fileStats;
fs.stat(file).then((stats) => {
    fileStats = stats;
    return fs.stat(dir);
}).then((dirStats) => {
    console.log('FileSize:', fileStats.size);
    console.log('FileModeHasRegularBit:', (fileStats.mode & 32768) === 32768);
    console.log('FileIsFile:', fileStats.isFile());
    console.log('FileHasBirthtimeMs:', typeof fileStats.birthtimeMs === 'number' && fileStats.birthtimeMs > 0);
    console.log('FileBirthtimeIsoLength:', fileStats.birthtime.toISOString().length);
    console.log('FileIsCharacterDevice:', fileStats.isCharacterDevice());

    console.log('DirModeHasDirectoryBit:', (dirStats.mode & 16384) === 16384);
    console.log('DirIsDirectory:', dirStats.isDirectory());
    console.log('DirHasCtimeMs:', typeof dirStats.ctimeMs === 'number' && dirStats.ctimeMs > 0);
    console.log('DirIsFIFO:', dirStats.isFIFO());

    syncFs.rmSync(file, { force: true });
    syncFs.rmSync(dir, { force: true, recursive: true });
}).catch((err) => {
    console.log('Error:', err && err.message);
    syncFs.rmSync(file, { force: true });
    syncFs.rmSync(dir, { force: true, recursive: true });
});
