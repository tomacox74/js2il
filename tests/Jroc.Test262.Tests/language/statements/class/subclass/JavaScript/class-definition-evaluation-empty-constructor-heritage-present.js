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
// Copyright (C) 2014 the V8 project authors. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.
/*---
es6id: 14.5.14
description: >
    10. If constructor is empty, then,
      a. If ClassHeritageopt is present, then
        i. Let constructor be the result of parsing the String "constructor(... args){ super (...args);}" using the syntactic grammar with the goal symbol MethodDefinition.
---*/
var args;

class A {
  constructor() {
    args = arguments;
  }
}

class B extends A {
  /*
    The missing constructor is created by the runtime:

    constructor(...args) {
      super(...args);
    }

   */
}

new B(0, 1, 2);


assert.sameValue(args[0], 0);
assert.sameValue(args[1], 1);
assert.sameValue(args[2], 2);

