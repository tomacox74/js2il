"use strict";

const path = require('path');

const parsed = path.parse('/home/user/dir/file.txt');
const parsedDir = String(parsed.dir || '').replace(/\\/g, '/');

console.log(parsed.base === 'file.txt');
console.log(parsed.ext === '.txt');
console.log(parsed.name === 'file');
console.log(parsed.root === '/' || parsed.root === '\\' || parsed.root === '');
console.log(parsedDir.endsWith('/home/user/dir'));

const f1 = path.format(parsed).replace(/\\/g, '/');
const f2 = path.format({ dir: '/tmp/demo', name: 'archive', ext: '.tar' }).replace(/\\/g, '/');
const f3 = path.format({ root: '/', base: 'index.js' }).replace(/\\/g, '/');

console.log(f1.endsWith('/home/user/dir/file.txt'));
console.log(f2.endsWith('/tmp/demo/archive.tar'));
console.log(f3.endsWith('/index.js'));
