"use strict";

// Function.prototype.call ( thisArg , ... args )

function f(a, b) {
  return this.x + '-' + a + '-' + b;
}

var obj = { x: 'T' };

console.log(f.call(obj, 'A', 'B'));
