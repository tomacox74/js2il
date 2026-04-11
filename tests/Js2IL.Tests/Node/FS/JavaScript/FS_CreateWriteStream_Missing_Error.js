"use strict";

const fs = require('fs');
const path = require('path');
const os = require('os');

const uniqueSuffix = `${Date.now()}-${Math.floor(Math.random() * 1000000000)}`;
const missingDir = path.join(os.tmpdir(), `js2il-fs-write-stream-missing-${uniqueSuffix}`, 'nested');
const file = path.join(missingDir, 'file.txt');
fs.rmSync(path.join(os.tmpdir(), `js2il-fs-write-stream-missing-${uniqueSuffix}`), { force: true, recursive: true });

const stream = fs.createWriteStream(file, { encoding: 'utf8' });

stream.on('error', (err) => {
    console.log('ErrorStartsWithENOENT:', /^ENOENT:/.test(err && err.message));
});

stream.on('close', () => {
    console.log('Closed:', true);
});

stream.write('nope');
