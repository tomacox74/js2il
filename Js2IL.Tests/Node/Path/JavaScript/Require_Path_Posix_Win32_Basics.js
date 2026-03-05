"use strict";

const path = require('path');

console.log(path.posix.sep === '/');
console.log(path.win32.sep === '\\');
console.log(path.posix.delimiter === ':');
console.log(path.win32.delimiter === ';');

console.log(path.posix.join('a', 'b') === 'a/b');
console.log(path.win32.join('a', 'b') === 'a\\b');

console.log(path.posix.normalize('a//b/../c') === 'a/c');
console.log(path.win32.normalize('a\\\\b\\..\\c') === 'a\\c');

console.log(path.posix.isAbsolute('/tmp') === true);
console.log(path.posix.isAbsolute('tmp') === false);
console.log(path.win32.isAbsolute('C:\\tmp') === true);
console.log(path.win32.isAbsolute('tmp') === false);

console.log(path.posix.normalize('a\\b') === 'a\\b');
console.log(path.posix.isAbsolute(' /tmp') === false);
console.log(path.posix.parse('file.txt').dir === '');
console.log(path.win32.parse('file.txt').dir === '');
