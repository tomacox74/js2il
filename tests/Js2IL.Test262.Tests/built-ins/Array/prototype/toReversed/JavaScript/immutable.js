// Copyright (C) 2021 Igalia, S.L. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
esid: sec-array.prototype.toreversed
description: >
  Array.prototype.toReversed does not mutate its this value
features: [change-array-by-copy]
includes: [compareArray.js]
---*/

function assert(value) { console.log(!!value); }
assert.sameValue = function(actual, expected) { console.log(Object.is(actual, expected)); };
assert.notSameValue = function(actual, unexpected) { console.log(!Object.is(actual, unexpected)); };
function compareArray(actual, expected) { if (actual == null || expected == null || actual.length !== expected.length) return false; for (let i = 0; i < actual.length; i++) { if (!Object.is(actual[i], expected[i])) return false; } return true; }
assert.compareArray = function(actual, expected) { console.log(compareArray(actual, expected)); };
assert.throws = function(expectedCtor, fn) { try { fn(); console.log(false); } catch (error) { console.log(error instanceof expectedCtor); } };
function verifyProperty(obj, name, desc) { var actual = Object.getOwnPropertyDescriptor(obj, name); if (actual === undefined) { console.log(false); return; } var ok = true; if (Object.prototype.hasOwnProperty.call(desc, 'value')) ok = ok && Object.is(actual.value, desc.value); if (Object.prototype.hasOwnProperty.call(desc, 'writable')) ok = ok && Object.is(actual.writable, desc.writable); if (Object.prototype.hasOwnProperty.call(desc, 'enumerable')) ok = ok && Object.is(actual.enumerable, desc.enumerable); if (Object.prototype.hasOwnProperty.call(desc, 'configurable')) ok = ok && Object.is(actual.configurable, desc.configurable); console.log(ok); }


var arr = [0, 1, 2];
arr.toReversed();

assert.compareArray(arr, [0, 1, 2]);
assert.notSameValue(arr.toReversed(), arr);
