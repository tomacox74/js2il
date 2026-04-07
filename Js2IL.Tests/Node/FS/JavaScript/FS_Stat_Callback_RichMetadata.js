"use strict";

const fs = require('fs');
const path = require('path');
const os = require('os');

const uniqueSuffix = `${Date.now()}-${Math.floor(Math.random() * 1000000000)}`;
const file = path.join(os.tmpdir(), `js2il-fs-stat-callback-rich-${uniqueSuffix}.txt`);

fs.rmSync(file, { force: true });
fs.writeFileSync(file, 'hello', 'utf8');

fs.stat(file, (err, stats) => {
    if (err) {
        console.log('Error:', err.message);
    } else {
        console.log('ModeHasRegularBit:', (stats.mode & 32768) === 32768);
        console.log('IsFile:', stats.isFile());
        console.log('IsDirectory:', stats.isDirectory());
        console.log('HasAtimeMs:', typeof stats.atimeMs === 'number' && stats.atimeMs > 0);
        console.log('AtimeIsoLength:', stats.atime.toISOString().length);
        console.log('IsSocket:', stats.isSocket());
    }

    fs.rmSync(file, { force: true });
});
