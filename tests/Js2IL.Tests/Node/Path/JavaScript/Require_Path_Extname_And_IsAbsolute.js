"use strict";

const path = require('path');

console.log(path.extname('index.html'));
console.log(path.extname('index.'));
console.log(path.extname('index'));
console.log(path.extname('.bashrc'));
console.log(path.extname('/a/b/archive.tar.gz'));

console.log(path.isAbsolute('/foo/bar'));
console.log(path.isAbsolute('foo/bar'));
