"use strict";

const fs = require('fs');
const path = require('path');

const file = path.join(__dirname, 'tmp-buffer.bin');
const out = Buffer.from([97, 98, 99]);
fs.writeFileSync(file, out);

const raw = fs.readFileSync(file);
console.log(Buffer.isBuffer(raw));
console.log(raw.length);
console.log(raw.toString());

const text = fs.readFileSync(file, 'utf8');
console.log(typeof text);
console.log(text);

fs.rmSync(file, { force: true });