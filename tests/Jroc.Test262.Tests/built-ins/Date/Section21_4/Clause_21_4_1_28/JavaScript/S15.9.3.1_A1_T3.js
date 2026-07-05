function Test262Error(message) {
    this.name = 'Test262Error';
    this.message = message || '';
}
Test262Error.prototype = Object.create(Error.prototype);
Test262Error.prototype.constructor = Test262Error;
function $ERROR(message) { throw new Test262Error(message); }
function $DONE(error) { if (error) { throw error; } }
function __test262SameValue(actual, expected) { return Object.is(actual, expected); }
function __test262FormatMessage(message, fallback) { return message || fallback || 'Assertion failed'; }
function assert(condition, message) {
    var passed = !!condition;
    console.log(passed);
    if (!passed) { throw new Error(__test262FormatMessage(message)); }
}
assert.sameValue = function(actual, expected, message) {
    var passed = __test262SameValue(actual, expected);
    console.log(passed);
    if (!passed) { throw new Error(__test262FormatMessage(message, 'Expected SameValue')); }
};
assert.notSameValue = function(actual, unexpected, message) {
    var passed = !__test262SameValue(actual, unexpected);
    console.log(passed);
    if (!passed) { throw new Error(__test262FormatMessage(message, 'Expected values to differ')); }
};
assert.strictEqual = assert.sameValue;
assert.notStrictEqual = assert.notSameValue;
assert.throws = function(expectedErrorConstructor, fn, message) {
    var passed = false;
    try { fn(); } catch (error) {
        passed = error instanceof expectedErrorConstructor ||
            (error && error.constructor === expectedErrorConstructor) ||
            (error && expectedErrorConstructor && error.name === expectedErrorConstructor.name);
    }
    console.log(passed);
    if (!passed) { throw new Error(__test262FormatMessage(message, 'Expected function to throw')); }
};
function compareArray(actual, expected) {
    if (!actual || !expected || actual.length !== expected.length) { return false; }
    for (var i = 0; i < actual.length; i++) {
        if (!__test262SameValue(actual[i], expected[i])) { return false; }
    }
    return true;
}
assert.compareArray = function(actual, expected, message) {
    var passed = compareArray(actual, expected);
    console.log(passed);
    if (!passed) { throw new Error(__test262FormatMessage(message, 'Expected arrays to match')); }
};
function verifyProperty(object, name, desc) {
    var actual = Object.getOwnPropertyDescriptor(object, name);
    var passed = !!actual;
    if (passed && Object.prototype.hasOwnProperty.call(desc, 'value')) { passed = __test262SameValue(actual.value, desc.value); }
    if (passed && Object.prototype.hasOwnProperty.call(desc, 'writable')) { passed = actual.writable === desc.writable; }
    if (passed && Object.prototype.hasOwnProperty.call(desc, 'enumerable')) { passed = actual.enumerable === desc.enumerable; }
    if (passed && Object.prototype.hasOwnProperty.call(desc, 'configurable')) { passed = actual.configurable === desc.configurable; }
    if (passed && Object.prototype.hasOwnProperty.call(desc, 'get')) { passed = actual.get === desc.get; }
    if (passed && Object.prototype.hasOwnProperty.call(desc, 'set')) { passed = actual.set === desc.set; }
    console.log(passed);
    if (!passed) { throw new Error('verifyProperty failed for ' + name); }
}
function assertRelativeDateMs(date, expectedMs) {
    var passed = Object.is(date.valueOf(), expectedMs);
    console.log(passed);
    if (!passed) { throw new Error('Expected date value ' + expectedMs); }
}
function isConstructor(fn) {
    try { Reflect.construct(Object, [], fn); return true; } catch (error) { return false; }
}

// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: |
    When Date is called as part of a new expression it is
    a constructor: it initializes the newly created object
esid: sec-date-year-month-date-hours-minutes-seconds-ms
description: 4 arguments, (year, month, date, hours)
---*/
assert.sameValue(
  typeof new Date(1899, 11, 31, 23),
  "object",
  'The value of `typeof new Date(1899, 11, 31, 23)` is expected to be "object"'
);

assert.notSameValue(
  new Date(1899, 11, 31, 23),
  undefined,
  'new Date(1899, 11, 31, 23) is expected to not equal ``undefined``'
);

var x13 = new Date(1899, 11, 31, 23);
assert.sameValue(typeof x13, "object", 'The value of `typeof x13` is expected to be "object"');

var x14 = new Date(1899, 11, 31, 23);
assert.notSameValue(x14, undefined, 'The value of x14 is expected to not equal ``undefined``');

assert.sameValue(
  typeof new Date(1899, 12, 1, 0),
  "object",
  'The value of `typeof new Date(1899, 12, 1, 0)` is expected to be "object"'
);

assert.notSameValue(
  new Date(1899, 12, 1, 0),
  undefined,
  'new Date(1899, 12, 1, 0) is expected to not equal ``undefined``'
);

var x23 = new Date(1899, 12, 1, 0);
assert.sameValue(typeof x23, "object", 'The value of `typeof x23` is expected to be "object"');

var x24 = new Date(1899, 12, 1, 0);
assert.notSameValue(x24, undefined, 'The value of x24 is expected to not equal ``undefined``');

assert.sameValue(
  typeof new Date(1900, 0, 1, 0),
  "object",
  'The value of `typeof new Date(1900, 0, 1, 0)` is expected to be "object"'
);

assert.notSameValue(new Date(1900, 0, 1, 0), undefined, 'new Date(1900, 0, 1, 0) is expected to not equal ``undefined``');

var x33 = new Date(1900, 0, 1, 0);
assert.sameValue(typeof x33, "object", 'The value of `typeof x33` is expected to be "object"');

var x34 = new Date(1900, 0, 1, 0);
assert.notSameValue(x34, undefined, 'The value of x34 is expected to not equal ``undefined``');

assert.sameValue(
  typeof new Date(1969, 11, 31, 23),
  "object",
  'The value of `typeof new Date(1969, 11, 31, 23)` is expected to be "object"'
);

assert.notSameValue(
  new Date(1969, 11, 31, 23),
  undefined,
  'new Date(1969, 11, 31, 23) is expected to not equal ``undefined``'
);

var x43 = new Date(1969, 11, 31, 23);
assert.sameValue(typeof x43, "object", 'The value of `typeof x43` is expected to be "object"');

var x44 = new Date(1969, 11, 31, 23);
assert.notSameValue(x44, undefined, 'The value of x44 is expected to not equal ``undefined``');

assert.sameValue(
  typeof new Date(1969, 12, 1, 0),
  "object",
  'The value of `typeof new Date(1969, 12, 1, 0)` is expected to be "object"'
);

assert.notSameValue(
  new Date(1969, 12, 1, 0),
  undefined,
  'new Date(1969, 12, 1, 0) is expected to not equal ``undefined``'
);

var x53 = new Date(1969, 12, 1, 0);
assert.sameValue(typeof x53, "object", 'The value of `typeof x53` is expected to be "object"');

var x54 = new Date(1969, 12, 1, 0);
assert.notSameValue(x54, undefined, 'The value of x54 is expected to not equal ``undefined``');

assert.sameValue(
  typeof new Date(1970, 0, 1, 0),
  "object",
  'The value of `typeof new Date(1970, 0, 1, 0)` is expected to be "object"'
);

assert.notSameValue(new Date(1970, 0, 1, 0), undefined, 'new Date(1970, 0, 1, 0) is expected to not equal ``undefined``');

var x63 = new Date(1970, 0, 1, 0);
assert.sameValue(typeof x63, "object", 'The value of `typeof x63` is expected to be "object"');

var x64 = new Date(1970, 0, 1, 0);
assert.notSameValue(x64, undefined, 'The value of x64 is expected to not equal ``undefined``');

assert.sameValue(
  typeof new Date(1999, 11, 31, 23),
  "object",
  'The value of `typeof new Date(1999, 11, 31, 23)` is expected to be "object"'
);

assert.notSameValue(
  new Date(1999, 11, 31, 23),
  undefined,
  'new Date(1999, 11, 31, 23) is expected to not equal ``undefined``'
);

var x73 = new Date(1999, 11, 31, 23);
assert.sameValue(typeof x73, "object", 'The value of `typeof x73` is expected to be "object"');

var x74 = new Date(1999, 11, 31, 23);
assert.notSameValue(x74, undefined, 'The value of x74 is expected to not equal ``undefined``');

assert.sameValue(
  typeof new Date(1999, 12, 1, 0),
  "object",
  'The value of `typeof new Date(1999, 12, 1, 0)` is expected to be "object"'
);

assert.notSameValue(
  new Date(1999, 12, 1, 0),
  undefined,
  'new Date(1999, 12, 1, 0) is expected to not equal ``undefined``'
);

var x83 = new Date(1999, 12, 1, 0);
assert.sameValue(typeof x83, "object", 'The value of `typeof x83` is expected to be "object"');

var x84 = new Date(1999, 12, 1, 0);
assert.notSameValue(x84, undefined, 'The value of x84 is expected to not equal ``undefined``');

assert.sameValue(
  typeof new Date(2000, 0, 1, 0),
  "object",
  'The value of `typeof new Date(2000, 0, 1, 0)` is expected to be "object"'
);

assert.notSameValue(new Date(2000, 0, 1, 0), undefined, 'new Date(2000, 0, 1, 0) is expected to not equal ``undefined``');

var x93 = new Date(2000, 0, 1, 0);
assert.sameValue(typeof x93, "object", 'The value of `typeof x93` is expected to be "object"');

var x94 = new Date(2000, 0, 1, 0);
assert.notSameValue(x94, undefined, 'The value of x94 is expected to not equal ``undefined``');

assert.sameValue(
  typeof new Date(2099, 11, 31, 23),
  "object",
  'The value of `typeof new Date(2099, 11, 31, 23)` is expected to be "object"'
);

assert.notSameValue(
  new Date(2099, 11, 31, 23),
  undefined,
  'new Date(2099, 11, 31, 23) is expected to not equal ``undefined``'
);

var x103 = new Date(2099, 11, 31, 23);
assert.sameValue(typeof x103, "object", 'The value of `typeof x103` is expected to be "object"');

var x104 = new Date(2099, 11, 31, 23);
assert.notSameValue(x104, undefined, 'The value of x104 is expected to not equal ``undefined``');

assert.sameValue(
  typeof new Date(2099, 12, 1, 0),
  "object",
  'The value of `typeof new Date(2099, 12, 1, 0)` is expected to be "object"'
);

assert.notSameValue(
  new Date(2099, 12, 1, 0),
  undefined,
  'new Date(2099, 12, 1, 0) is expected to not equal ``undefined``'
);

var x113 = new Date(2099, 12, 1, 0);
assert.sameValue(typeof x113, "object", 'The value of `typeof x113` is expected to be "object"');

var x114 = new Date(2099, 12, 1, 0);
assert.notSameValue(x114, undefined, 'The value of x114 is expected to not equal ``undefined``');

assert.sameValue(
  typeof new Date(2100, 0, 1, 0),
  "object",
  'The value of `typeof new Date(2100, 0, 1, 0)` is expected to be "object"'
);

assert.notSameValue(new Date(2100, 0, 1, 0), undefined, 'new Date(2100, 0, 1, 0) is expected to not equal ``undefined``');

var x123 = new Date(2100, 0, 1, 0);
assert.sameValue(typeof x123, "object", 'The value of `typeof x123` is expected to be "object"');

var x124 = new Date(2100, 0, 1, 0);
assert.notSameValue(x124, undefined, 'The value of x124 is expected to not equal ``undefined``');
