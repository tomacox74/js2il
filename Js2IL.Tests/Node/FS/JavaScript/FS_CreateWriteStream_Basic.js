"use strict";

const fs = require('fs');
const path = require('path');
const os = require('os');

const uniqueSuffix = `${Date.now()}-${Math.floor(Math.random() * 1000000000)}`;
const file = path.join(os.tmpdir(), `js2il-fs-write-stream-${uniqueSuffix}.txt`);
fs.rmSync(file, { force: true });

const stream = fs.createWriteStream(file, { encoding: 'utf8' });

stream.on('finish', () => {
    console.log('FileText:', fs.readFileSync(file, 'utf8'));
    fs.rmSync(file, { force: true });
});

stream.on('close', () => {
    console.log('Closed:', true);
});

stream.on('error', (err) => {
    console.log('Error:', err && err.message);
    fs.rmSync(file, { force: true });
});

stream.write('hello ');
stream.end('world');
