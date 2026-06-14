// Copyright (C) 2015 André Bargull. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
es6id: 25.4.1.5.1
description: The [[Extensible]] slot of GetCapabilitiesExecutor functions
info: |
  17 ECMAScript Standard Built-in Objects:
    Unless specified otherwise, the [[Extensible]] internal slot
    of a built-in object initially has the value true.
---*/

function assert(value) { console.log(!!value); }
assert.sameValue = function (actual, expected) { console.log(Object.is(actual, expected)); };
assert.notSameValue = function (actual, unexpected) { console.log(!Object.is(actual, unexpected)); };

var executorFunction;

function NotPromise(executor) {
  executorFunction = executor;
  executor(function() {}, function() {});
}
Promise.resolve.call(NotPromise);

assert(Object.isExtensible(executorFunction));
