function Test262Error(message) {
    this.message = message || "";
    this.name = "Test262Error";
}
Test262Error.prototype = Object.create(Error.prototype);
Test262Error.prototype.constructor = Test262Error;

function __test262SameValue(actual, expected) {
    return Object.is(actual, expected);
}

var assert = function assert(value, message) {
    var passed = !!value;
    console.log(passed);
    if (!passed) {
        throw new Error(message || "Assertion failed");
    }
};

assert.sameValue = function(actual, expected, message) {
    var passed = __test262SameValue(actual, expected);
    console.log(passed);
    if (!passed) {
        throw new Error(message || "Expected SameValue");
    }
};

assert.throws = function(expectedErrorConstructor, fn, message) {
    var passed = false;
    try {
        fn();
    } catch (error) {
        passed = error instanceof expectedErrorConstructor ||
            (error && error.constructor === expectedErrorConstructor) ||
            (error && expectedErrorConstructor && error.name === expectedErrorConstructor.name);
    }
    console.log(passed);
    if (!passed) {
        throw new Error(message || "Expected function to throw");
    }
};
// Copyright (C) 2016 the V8 project authors. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.
/*---
esid: sec-runtime-semantics-classdefinitionevaluation
description: >
  Attempting to call `super()` in a null-extending class throws a TypeError,
  because %FunctionPrototype% cannot be called as constructor function.
info: |
  Runtime Semantics: ClassDefinitionEvaluation

  [...]
  5. If ClassHeritageopt is not present, then
     [...]
  6. Else,
     [...]
     b. Let superclass be the result of evaluating ClassHeritage.
     [...]
     e. If superclass is null, then
        [...]
        ii. Let constructorParent be the intrinsic object %FunctionPrototype%.
  [...]
  15. Let constructorInfo be the result of performing DefineMethod for constructor with arguments proto and constructorParent as the optional functionPrototype argument.
  [...]

  12.3.5.1 Runtime Semantics: Evaluation

  SuperCall : super Arguments

  [...]
  3. Let func be ! GetSuperConstructor().
  4. Let argList be ? ArgumentListEvaluation of Arguments.
  5. If IsConstructor(func) is false, throw a TypeError exception.
  [...]
---*/

var unreachable = 0;
var reachable = 0;

class C extends null {
  constructor() {
    reachable += 1;
    super();
    unreachable += 1;
  }
}

assert.throws(TypeError, function() {
  new C();
});

assert.sameValue(reachable, 1);
assert.sameValue(unreachable, 0);
