// Copyright (C) 2017 Josh Wolfe. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.
/*---
description: Non-strict equality comparison of BigInt and miscellaneous primitive values
esid: sec-equality-operators-runtime-semantics-evaluation
info: |
  EqualityExpression : EqualityExpression == RelationalExpression
    ...
    5. Return the result of performing Abstract Equality Comparison rval == lval.

features: [BigInt, Symbol]
---*/
console.log(Object.is(0n == undefined, false));
console.log(Object.is(undefined == 0n, false));
console.log(Object.is(1n == undefined, false));
console.log(Object.is(undefined == 1n, false));
console.log(Object.is(0n == null, false));
console.log(Object.is(null == 0n, false));
console.log(Object.is(1n == null, false));
console.log(Object.is(null == 1n, false));
console.log(Object.is(0n == Symbol('1'), false));
console.log(Object.is(Symbol('1') == 0n, false));
console.log(Object.is(1n == Symbol('1'), false));
console.log(Object.is(Symbol('1') == 1n, false));
