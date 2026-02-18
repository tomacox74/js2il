"use strict";

const path = require('path');
const sep = path.sep;
const samplePath = sep + ['home', 'user', 'dir', 'file.txt'].join(sep);

const parsed = path.parse(samplePath);
const parsedDir = String(parsed.dir || '');

console.log(parsed.base === 'file.txt');
console.log(parsed.ext === '.txt');
console.log(parsed.name === 'file');
console.log(parsed.root === sep || parsed.root === '');
console.log(parsedDir.endsWith(sep + ['home', 'user', 'dir'].join(sep)));

const f1 = path.format(parsed);
const f2 = path.format({ dir: sep + ['tmp', 'demo'].join(sep), name: 'archive', ext: '.tar' });
const f3 = path.format({ root: sep, base: 'index.js' });

console.log(f1.endsWith(sep + ['home', 'user', 'dir', 'file.txt'].join(sep)));
console.log(f2.endsWith(sep + ['tmp', 'demo', 'archive.tar'].join(sep)));
console.log(f3.endsWith(sep + 'index.js'));
