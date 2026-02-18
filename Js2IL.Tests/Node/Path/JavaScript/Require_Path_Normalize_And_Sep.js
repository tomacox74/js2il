"use strict";

const path = require('path');

const n1 = path.normalize('a//b/../c').replace(/\\/g, '/');
const n2 = path.normalize('/tmp//x/./y').replace(/\\/g, '/');

console.log(n1 === 'a/c');
console.log(n2 === '/tmp/x/y');
console.log(path.sep === '/' || path.sep === '\\');
