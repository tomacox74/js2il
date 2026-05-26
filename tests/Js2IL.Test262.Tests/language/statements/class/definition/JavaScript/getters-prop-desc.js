function Test262Error(message) {
    this.message = message || "";
    this.name = "Test262Error";
}
Test262Error.prototype = Object.create(Error.prototype);
Test262Error.prototype.constructor = Test262Error;

var assert = function assert(value, message) {
    var passed = !!value;
    console.log(passed);
    if (!passed) {
        throw new Error(message || "Assertion failed");
    }
};

assert.sameValue = function(actual, expected, message) {
    var passed = Object.is(actual, expected);
    console.log(passed);
    if (!passed) {
        throw new Error(message || "Expected SameValue");
    }
};

assert.throws = function(expectedErrorConstructor, fn, message) {
    var passed = false;
    try {
        fn();
    } catch (error) {
        passed = error instanceof expectedErrorConstructor ||
            (error && error.constructor === expectedErrorConstructor) ||
            (error && expectedErrorConstructor && error.name === expectedErrorConstructor.name);
    }
    console.log(passed);
    if (!passed) {
        throw new Error(message || "Expected function to throw");
    }
};

function verifyProperty(object, name, expected) {
    var desc = Object.getOwnPropertyDescriptor(object, name);
    if ('value' in expected) {
        assert.sameValue(desc.value, expected.value, 'descriptor value');
    }
    if ('writable' in expected) {
        assert.sameValue(desc.writable, expected.writable, 'descriptor writable');
    }
    if ('enumerable' in expected) {
        assert.sameValue(desc.enumerable, expected.enumerable, 'descriptor enumerable');
    }
    if ('configurable' in expected) {
        assert.sameValue(desc.configurable, expected.configurable, 'descriptor configurable');
    }
}
// Copyright (C) 2014 the V8 project authors. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.
/*---
esid: sec-class-definitions
es6id: 14.5
description: Class methods - "get" accessors
includes: [propertyHelper.js]
---*/

function assertGetterDescriptor(object, name) {
  var desc = Object.getOwnPropertyDescriptor(object, name);
  verifyProperty(object, name, {
    enumerable: false,
    configurable: true,
  });
  assert.sameValue(typeof desc.get, 'function', "`typeof desc.get` is `'function'`");
  assert.sameValue('prototype' in desc.get, false, "The result of `'prototype' in desc.get` is `false`");
  assert.sameValue(desc.set, undefined, "The value of `desc.set` is `undefined`");
}

class C {
  get x() { return 1; }
  static get staticX() { return 2; }
  get y() { return 3; }
  static get staticY() { return 4; }
}

assert.sameValue(new C().x, 1, "The value of `new C().x` is `1`. Defined as `get x() { return 1; }`");
assert.sameValue(C.staticX, 2, "The value of `C.staticX` is `2`. Defined as `static get staticX() { return 2; }`");
assert.sameValue(new C().y, 3, "The value of `new C().y` is `3`. Defined as `get y() { return 3; }`");
assert.sameValue(C.staticY, 4, "The value of `C.staticY` is `4`. Defined as `static get staticY() { return 4; }`");

assertGetterDescriptor(C.prototype, 'x');
assertGetterDescriptor(C.prototype, 'y');
assertGetterDescriptor(C, 'staticX');
assertGetterDescriptor(C, 'staticY');
