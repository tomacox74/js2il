// Copyright (C) 2015 the V8 project authors. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.
/*---
esid: sec-set-constructor
description: >
    Set ( [ iterable ] )

    When the Set function is called with optional argument iterable the following steps are taken:

    ...
    7. Else,
      a. Let adder be Get(set, "add").
      b. ReturnIfAbrupt(adder).
      c. If IsCallable(adder) is false, throw a TypeError exception.
      d. Let iter be GetIterator(iterable).
      e. ReturnIfAbrupt(iter).
    ...
    9. Repeat
      a. Let next be IteratorStep(iter).
      b. ReturnIfAbrupt(next).
      c. If next is false, return set.
      d. Let nextValue be IteratorValue(next).
      e. ReturnIfAbrupt(nextValue).
      f. Let status be Call(adder, set, «nextValue.[[value]]»).
      g. If status is an abrupt completion, return IteratorClose(iter, status).

---*/

function assert(value) { console.log(!!value); }
assert.sameValue = function (actual, expected) { console.log(Object.is(actual, expected)); };
assert.notSameValue = function (actual, unexpected) { console.log(!Object.is(actual, unexpected)); };

var s = new Set([1, 2]);

assert.sameValue(s.size, 2, "The value of `s.size` is `2`");
