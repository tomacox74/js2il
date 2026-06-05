// Copyright (C) 2021 Igalia, S.L. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
esid: sec-array.prototype.tosorted
description: >
  Array.prototype.toSorted does not mutate its this value
features: [change-array-by-copy]
includes: [compareArray.js]
---*/

function assert(value) { console.log(!!value); }
assert.sameValue = function(actual, expected) { console.log(Object.is(actual, expected)); };
assert.notSameValue = function(actual, unexpected) { console.log(!Object.is(actual, unexpected)); };
function compareArray(actual, expected) { if (actual == null || expected == null || actual.length !== expected.length) return false; for (let i = 0; i < actual.length; i++) { if (!Object.is(actual[i], expected[i])) return false; } return true; }
assert.compareArray = function(actual, expected) { console.log(compareArray(actual, expected)); };


var arr = [2, 0, 1];
arr.toSorted();

assert.compareArray(arr, [2, 0, 1]);
assert.notSameValue(arr.toSorted(), arr);
