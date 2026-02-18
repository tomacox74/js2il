"use strict";

const path = require('path');

console.log(path.normalize('a//b/../c'));
console.log(path.normalize('/tmp//x/./y'));
console.log(path.sep);
