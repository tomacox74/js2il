// Copyright (C) 2014 the V8 project authors. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.
/*---
es6id: 14.5
description: >
    class subclass binding
---*/
function Test262Error(message) {
    this.name = 'Test262Error';
    this.message = message || '';
}

Test262Error.prototype = Object.create(Error.prototype);
Test262Error.prototype.constructor = Test262Error;

function $ERROR(message) {
    throw new Test262Error(message);
}

function $DONE(error) {
    if (error) {
        throw error;
    }
}

function __test262SameValue(actual, expected) {
    return Object.is(actual, expected);
}

function __test262FormatMessage(message, fallback) {
    return message || fallback || "Assertion failed";
}

function assert(condition, message) {
    var passed = !!condition;
    console.log(passed);
    if (!passed) {
        throw new Error(__test262FormatMessage(message));
    }
}

assert.sameValue = function(actual, expected, message) {
    var passed = __test262SameValue(actual, expected);
    console.log(passed);
    if (!passed) {
        throw new Error(__test262FormatMessage(message, "Expected SameValue"));
    }
};

assert.notSameValue = function(actual, unexpected, message) {
    var passed = !__test262SameValue(actual, unexpected);
    console.log(passed);
    if (!passed) {
        throw new Error(__test262FormatMessage(message, "Expected values to differ"));
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
        throw new Error(__test262FormatMessage(message, "Expected function to throw"));
    }
};

function compareArray(actual, expected) {
    if (!actual || !expected || actual.length !== expected.length) {
        return false;
    }

    for (var i = 0; i < actual.length; i++) {
        if (!__test262SameValue(actual[i], expected[i])) {
            return false;
        }
    }

    return true;
}

assert.compareArray = function(actual, expected, message) {
    var passed = compareArray(actual, expected);
    console.log(passed);
    if (!passed) {
        throw new Error(__test262FormatMessage(message, "Expected arrays to match"));
    }
};

function verifyProperty(object, name, desc) {
    var actual = Object.getOwnPropertyDescriptor(object, name);
    var passed = !!actual;

    if (passed && Object.prototype.hasOwnProperty.call(desc, 'value')) {
        passed = __test262SameValue(actual.value, desc.value);
    }

    if (passed && Object.prototype.hasOwnProperty.call(desc, 'writable')) {
        passed = actual.writable === desc.writable;
    }

    if (passed && Object.prototype.hasOwnProperty.call(desc, 'enumerable')) {
        passed = actual.enumerable === desc.enumerable;
    }

    if (passed && Object.prototype.hasOwnProperty.call(desc, 'configurable')) {
        passed = actual.configurable === desc.configurable;
    }

    if (passed && Object.prototype.hasOwnProperty.call(desc, 'get')) {
        passed = actual.get === desc.get;
    }

    if (passed && Object.prototype.hasOwnProperty.call(desc, 'set')) {
        passed = actual.set === desc.set;
    }

    console.log(passed);
    if (!passed) {
        throw new Error('verifyProperty failed for ' + name);
    }
}
class Base {
  constructor(x, y) {
    this.x = x;
    this.y = y;
  }
}

var obj = {};
class Subclass extends Base {
  constructor(x, y) {
    super(x,y);
    assert.sameValue(this !== obj, true, "The result of `this !== obj` is `true`");
  }
}

var f = Subclass.bind(obj);
assert.throws(TypeError, function () { f(1, 2); });
var s = new f(1, 2);
assert.sameValue(s.x, 1, "The value of `s.x` is `1`");
assert.sameValue(s.y, 2, "The value of `s.y` is `2`");
assert.sameValue(
  Object.getPrototypeOf(s),
  Subclass.prototype,
  "`Object.getPrototypeOf(s)` returns `Subclass.prototype`"
);

var s1 = new f(1);
assert.sameValue(s1.x, 1, "The value of `s1.x` is `1`");
assert.sameValue(s1.y, undefined, "The value of `s1.y` is `undefined`");
assert.sameValue(
  Object.getPrototypeOf(s1),
  Subclass.prototype,
  "`Object.getPrototypeOf(s1)` returns `Subclass.prototype`"
);

var g = Subclass.bind(obj, 1);
assert.throws(TypeError, function () { g(8); });
var s2 = new g(8);
assert.sameValue(s2.x, 1, "The value of `s2.x` is `1`");
assert.sameValue(s2.y, 8, "The value of `s2.y` is `8`");
assert.sameValue(
  Object.getPrototypeOf(s),
  Subclass.prototype,
  "`Object.getPrototypeOf(s)` returns `Subclass.prototype`"
);
