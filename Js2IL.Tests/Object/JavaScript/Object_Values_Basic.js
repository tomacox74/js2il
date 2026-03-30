"use strict";

// Object.values ( O )

var o = { a: 1, b: 2, c: 3 };

Object.defineProperty(o, 'hidden', {
  value: 4,
  enumerable: false,
  configurable: true,
  writable: true,
});

var values = Object.values(o);

console.log('length=' + values.length);
console.log('values=' + values.join(','));
