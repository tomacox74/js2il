"use strict";

const fs = require('fs');
const path = require('path');
const os = require('os');

const uniqueSuffix = `${Date.now()}-${Math.floor(Math.random() * 1000000000)}`;
const file = path.join(os.tmpdir(), `js2il-fs-read-stream-${uniqueSuffix}.txt`);
fs.rmSync(file, { force: true });
fs.writeFileSync(file, 'hello world', 'utf8');

const chunks = [];
const stream = fs.createReadStream(file, { encoding: 'utf8', highWaterMark: 5 });

stream.on('data', (chunk) => {
    console.log('Chunk:', chunk);
    chunks.push(chunk);
});

stream.on('end', () => {
    console.log('Combined:', chunks.join('|'));
    fs.rmSync(file, { force: true });
});

stream.on('error', (err) => {
    console.log('Error:', err && err.message);
    fs.rmSync(file, { force: true });
});
