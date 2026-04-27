// Copyright 2015 Microsoft Corporation. All rights reserved.
// This code is governed by the license found in the LICENSE file.
/*---
esid: sec-array.from
description: Map function without thisArg on non strict mode
info: |
  22.1.2.1 Array.from ( items [ , mapfn [ , thisArg ] ] )

  ...
  10. Let len be ToLength(Get(arrayLike, "length")).
  11. ReturnIfAbrupt(len).
  12. If IsConstructor(C) is true, then
    a. Let A be Construct(C, «len»).
  13. Else,
    b. Let A be ArrayCreate(len).
  14. ReturnIfAbrupt(A).
  15. Let k be 0.
  16. Repeat, while k < len
    a. Let Pk be ToString(k).
    b. Let kValue be Get(arrayLike, Pk).
    c. ReturnIfAbrupt(kValue).
    d. If mapping is true, then
      i. Let mappedValue be Call(mapfn, T, «kValue, k»).
  ...
flags: [noStrict]
---*/

var list = {
  '0': 41,
  '1': 42,
  '2': 43,
  length: 3
};
var calls = [];

function mapFn(value) {
  calls.push({
    args: arguments,
    thisArg: this
  });
  return value * 2;
}

var result = Array.from(list, mapFn);

console.log(Object.is(result.length, 3));
console.log(Object.is(result[0], 82));
console.log(Object.is(result[1], 84));
console.log(Object.is(result[2], 86));

console.log(Object.is(calls.length, 3));

console.log(Object.is(calls[0].args.length, 2));
console.log(Object.is(calls[0].args[0], 41));
console.log(Object.is(calls[0].args[1], 0));
console.log(Object.is(calls[0].thisArg, this));

console.log(Object.is(calls[1].args.length, 2));
console.log(Object.is(calls[1].args[0], 42));
console.log(Object.is(calls[1].args[1], 1));
console.log(Object.is(calls[1].thisArg, this));

console.log(Object.is(calls[2].args.length, 2));
console.log(Object.is(calls[2].args[0], 43));
console.log(Object.is(calls[2].args[1], 2));
console.log(Object.is(calls[2].thisArg, this));
