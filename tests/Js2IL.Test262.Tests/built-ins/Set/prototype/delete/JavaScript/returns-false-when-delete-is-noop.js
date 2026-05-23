// Copyright (C) 2015 the V8 project authors. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.
/*---
esid: sec-set.prototype.delete
description: >
    Set.prototype.delete ( value )

    ...
    6. Return false.

---*/

function assert(value) { console.log(!!value); }
assert.sameValue = function (actual, expected) { console.log(Object.is(actual, expected)); };
assert.notSameValue = function (actual, unexpected) { console.log(!Object.is(actual, unexpected)); };

var s = new Set();

assert.sameValue(s.delete(1), false, "`s.delete(1)` returns `false`");
