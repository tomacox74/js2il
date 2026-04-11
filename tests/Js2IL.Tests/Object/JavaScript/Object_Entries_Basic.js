"use strict";

// Object.entries ( O )

var o = { a: 1, b: 2, c: 3 };

Object.defineProperty(o, 'hidden', {
  value: 4,
  enumerable: false,
  configurable: true,
  writable: true,
});

var entries = Object.entries(o);

console.log('length=' + entries.length);
for (var i = 0; i < entries.length; i++) {
  console.log(entries[i][0] + '=' + entries[i][1]);
}
