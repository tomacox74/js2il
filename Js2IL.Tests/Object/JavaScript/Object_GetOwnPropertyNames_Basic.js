"use strict";

// Object.getOwnPropertyNames ( O )

var o = { a: 1 };

Object.defineProperty(o, 'hidden', {
  value: 2,
  enumerable: false,
  configurable: true,
  writable: true,
});

var names = Object.getOwnPropertyNames(o);

var foundA = false;
var foundHidden = false;
for (var i = 0; i < names.length; i++) {
  if (names[i] === 'a') foundA = true;
  if (names[i] === 'hidden') foundHidden = true;
}

console.log('len=' + names.length);
console.log('has_a=' + foundA);
console.log('has_hidden=' + foundHidden);
