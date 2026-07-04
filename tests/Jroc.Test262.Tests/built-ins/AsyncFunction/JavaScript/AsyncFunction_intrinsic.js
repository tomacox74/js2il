// Copyright 2016 Microsoft, Inc. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
author: Brian Terlson <brian.terlson@microsoft.com>
esid: sec-async-function-objects
description: >
  %AsyncFunction% exists and is a function
---*/

function assert(value) { console.log(!!value); }
assert.sameValue = function (actual, expected) { console.log(Object.is(actual, expected)); };
assert.notSameValue = function (actual, unexpected) { console.log(!Object.is(actual, unexpected)); };

var AsyncFunction = async function foo() {}.constructor;
assert.sameValue(typeof AsyncFunction, "function");
