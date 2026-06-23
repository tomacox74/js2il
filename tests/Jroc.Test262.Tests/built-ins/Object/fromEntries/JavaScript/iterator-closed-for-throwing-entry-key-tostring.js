// Copyright (C) 2018 Kevin Gibbons. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
esid: sec-object.fromentries
description: Closes iterators when toString on a key throws.
info: |
  Object.fromEntries ( iterable )

  ...
  4. Let stepsDefine be the algorithm steps defined in CreateDataPropertyOnObject Functions.
  5. Let adder be CreateBuiltinFunction(stepsDefine, « »).
  6. Return ? AddEntriesFromIterable(obj, iterable, adder).

  AddEntriesFromIterable ( target, iterable, adder )

  ...
  4. Repeat,
    ...
    e. Let k be Get(nextItem, "0").
    f. If k is an abrupt completion, return ? IteratorClose(iteratorRecord, k).

features: [Symbol.iterator, Object.fromEntries]
---*/

function DummyError() {}

var returned = false;
var iterable = {
  [Symbol.iterator]: function() {
    var advanced = false;
    return {
      next: function() {
        if (advanced) {
          throw new Error('should only advance once');
        }
        advanced = true;
        return {
          done: false,
          value: {
            0: {
              toString: function() {
                throw new DummyError();
              },
            },
          },
        };
      },
      return: function() {
        if (returned) {
          throw new Error('should only return once');
        }
        returned = true;
      },
    };
  },
};

var threwDummyError = false;
try {
  Object.fromEntries(iterable);
} catch (error) {
  threwDummyError = error instanceof DummyError;
}

console.log(threwDummyError);
console.log(returned);
