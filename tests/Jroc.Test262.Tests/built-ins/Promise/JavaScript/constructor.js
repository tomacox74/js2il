// Copyright (C) 2019 Aleksey Shvayka. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.
/*---
esid: sec-promise-executor
description: >
  The Promise constructor is a built-in function
---*/

function assert(value) { console.log(!!value); }
assert.sameValue = function (actual, expected) { console.log(Object.is(actual, expected)); };
assert.notSameValue = function (actual, unexpected) { console.log(!Object.is(actual, unexpected)); };

assert.sameValue(typeof Promise, 'function');
