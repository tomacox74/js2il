"use strict";

const path = require('path');
const sep = path.sep;

const n1 = path.normalize('a' + sep + sep + 'b' + sep + '..' + sep + 'c');
const n2 = path.normalize(sep + 'tmp' + sep + sep + 'x' + sep + '.' + sep + 'y');

console.log(n1 === ('a' + sep + 'c'));
console.log(n2 === (sep + ['tmp', 'x', 'y'].join(sep)));
console.log(path.sep === sep);
