// Copyright (C) 2017 the V8 project authors. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

var $MAX_ITERATIONS = typeof $MAX_ITERATIONS === "undefined" ? 100000 : $MAX_ITERATIONS;
var assert = function assert(value) {
    console.log(!!value);
};
var obj;
var sample = {};

obj = {
  ['__proto__']: sample
};
assert.sameValue(
  Object.getPrototypeOf(obj),
  Object.prototype,
  'does not change the object prototype (ordinary object)'
);
assert(
  obj.hasOwnProperty('__proto__'),
  'computed __proto__ property is set as an own property (ordinary object)'
);
assert.sameValue(
  obj.__proto__,
  sample,
  'value is properly defined (ordinary object)'
);

obj = {
  ['__proto__']: null
};
assert.sameValue(
  Object.getPrototypeOf(obj),
  Object.prototype,
  'does not change the object prototype (null)'
);
assert(
  obj.hasOwnProperty('__proto__'),
  'computed __proto__ property is set as an own property (null)'
);
assert.sameValue(
  obj.__proto__,
  null,
  'value is properly defined (null)'
);

obj = {
  ['__proto__']: undefined
};
assert.sameValue(
  Object.getPrototypeOf(obj),
  Object.prototype,
  'does not change the object prototype (undefined)'
);
assert(
  obj.hasOwnProperty('__proto__'),
  'computed __proto__ property is set as an own property (undefined)'
);
assert.sameValue(
  obj.__proto__,
  undefined,
  'value is properly defined (undefined)'
);

var func = function() {};
obj = {
  ['__proto__']: func
};
assert.sameValue(
  Object.getPrototypeOf(obj),
  Object.prototype,
  'does not change the object prototype (func)'
);
assert(
  obj.hasOwnProperty('__proto__'),
  'computed __proto__ property is set as an own property (func)'
);
assert.sameValue(
  obj.__proto__,
  func,
  'value is properly defined (func)'
);

var symbol = Symbol('Leo');
obj = {
  ['__proto__']: symbol
};
assert.sameValue(
  Object.getPrototypeOf(obj),
  Object.prototype,
  'does not change the object prototype (symbol)'
);
assert(
  obj.hasOwnProperty('__proto__'),
  'computed __proto__ property is set as an own property (symbol)'
);
assert.sameValue(
  obj.__proto__,
  symbol,
  'value is properly defined (symbol)'
);

obj = {
  ['__proto__']: 42
};
assert.sameValue(
  Object.getPrototypeOf(obj),
  Object.prototype,
  'does not change the object prototype (number)'
);
assert(
  obj.hasOwnProperty('__proto__'),
  'computed __proto__ property is set as an own property (number)'
);
assert.sameValue(
  obj.__proto__,
  42,
  'value is properly defined (number)'
);

obj = {
  ['__proto__']: ''
};
assert.sameValue(
  Object.getPrototypeOf(obj),
  Object.prototype,
  'does not change the object prototype (string)'
);
assert(
  obj.hasOwnProperty('__proto__'),
  'computed __proto__ property is set as an own property (string)'
);
assert.sameValue(
  obj.__proto__,
  '',
  'value is properly defined (string)'
);

obj = {
  ['__proto__']: false
};
assert.sameValue(
  Object.getPrototypeOf(obj),
  Object.prototype,
  'does not change the object prototype (boolean)'
);
assert(
  obj.hasOwnProperty('__proto__'),
  'computed __proto__ property is set as an own property (boolean)'
);
assert.sameValue(
  obj.__proto__,
  false,
  'value is properly defined (boolean)'
);
