"use strict";

const fs = require('fs');
const path = require('path');
const os = require('os');

const uniqueSuffix = `${Date.now()}-${Math.floor(Math.random() * 1000000000)}`;
const file = path.join(os.tmpdir(), `js2il-fs-read-stream-missing-${uniqueSuffix}.txt`);
fs.rmSync(file, { force: true });

const stream = fs.createReadStream(file, { encoding: 'utf8' });

stream.on('error', (err) => {
    console.log('ErrorStartsWithENOENT:', /^ENOENT:/.test(err && err.message));
});

stream.on('close', () => {
    console.log('Closed:', true);
});
