// Copyright 2019 Google, Inc.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.
/*---
esid: prod-OptionalExpression
description: >
  optional chain on call expression
info: |
  Left-Hand-Side Expressions
    OptionalExpression:
      CallExpression OptionalChain
features: [optional-chaining]
---*/

// CallExpression CoverCallExpressionAndAsyncArrowHead
function fn () {
  return {a: 33};
};
const obj = {
  fn () {
    return 44;
  }
}
console.log(Object.is(33, fn()?.a));
console.log(Object.is(undefined, fn()?.b));
console.log(Object.is(44, obj?.fn()));

// CallExpression SuperCall
class A {}
class B extends A {
  constructor () {
    console.log(Object.is(undefined, super()?.a));
  }
}
new B();

// CallExpression Arguments
function fn2 () {
  return () => {
    return {a: 66};
  };
}
function fn3 () {
  return () => {
    return null;
  };
}
console.log(Object.is(66, fn2()()?.a));
console.log(Object.is(undefined, fn3()()?.a));

// CallExpression [Expression]
function fn4 () {
  return [{a: 77}];
}
function fn5 () {
  return [];
}
console.log(Object.is(77, fn4()[0]?.a));
console.log(Object.is(undefined, fn5()[0]?.a));

// CallExpression .IdentifierName
function fn6 () {
  return {
    a: {
      b: 88
    }
  };
}
console.log(Object.is(88, fn6().a?.b));
console.log(Object.is(undefined, fn6().b?.c));

// CallExpression TemplateLiteral
function fn7 () {
  return () => {};
}
console.log(Object.is(undefined, fn7()`hello`?.a));
