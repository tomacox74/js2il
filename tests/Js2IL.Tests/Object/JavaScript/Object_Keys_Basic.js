"use strict";

// Object.keys ( O )

var o = { a: 1, b: 2, c: 3 };

Object.defineProperty(o, 'hidden', {
  value: 4,
  enumerable: false,
  configurable: true,
  writable: true,
});

var keys = Object.keys(o);

console.log('length=' + keys.length);
console.log('keys=' + keys.join(','));
