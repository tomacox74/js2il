// Copyright (C) 2017 the V8 project authors. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

var $MAX_ITERATIONS = typeof $MAX_ITERATIONS === "undefined" ? 100000 : $MAX_ITERATIONS;
var assert = function assert(value) {
    console.log(!!value);
};
var obj;
var proto = {};
var ownProp = {};

obj = {
    __proto__: proto,
    ['__proto__']: {},
    ['__proto__']: ownProp
};

assert.sameValue(
    Object.getPrototypeOf(obj),
    proto,
    'prototype is defined'
);

assert(
    Object.prototype.hasOwnProperty.call(obj, '__proto__'),
    'has own property __proto__'
);

assert.sameValue(
    obj.__proto__,
    ownProp,
    'own property value'
);
