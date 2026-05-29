function Test262Error(message) {
    this.message = message || "";
    this.name = "Test262Error";
}
Test262Error.prototype = Object.create(Error.prototype);
Test262Error.prototype.constructor = Test262Error;

var $MAX_ITERATIONS = typeof $MAX_ITERATIONS === "undefined" ? 100000 : $MAX_ITERATIONS;

var assert = function assert(value, message) {
    var passed = !!value;
    console.log(passed);
    if (!passed) {
        throw new Error(message || "Assertion failed");
    }
};

assert.sameValue = function(actual, expected, message) {
    var passed = Object.is(actual, expected);
    console.log(passed);
    if (!passed) {
        throw new Error(message || "Expected SameValue");
    }
};

assert.notSameValue = function(actual, unexpected, message) {
    var passed = !Object.is(actual, unexpected);
    console.log(passed);
    if (!passed) {
        throw new Error(message || "Expected different values");
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

assert.compareArray = function(actual, expected, message) {
    var passed = actual.length === expected.length && actual.every(function(value, index) {
        return Object.is(value, expected[index]);
    });
    console.log(passed);
    if (!passed) {
        throw new Error(message || "Expected arrays to be equal");
    }
};

// This file was procedurally generated from the following sources:
// - src/computed-property-names/computed-property-name-from-math.case
// - src/computed-property-names/evaluation/class-expression-accessors.template
/*---
description: Computed property name from math (ComputedPropertyName in ClassExpression)
esid: prod-ComputedPropertyName
features: [computed-property-names, exponentiation]
flags: [generated]
info: |
    ClassExpression:
      classBindingIdentifier opt ClassTail

    ClassTail:
      ClassHeritage opt { ClassBody opt }

    ClassBody:
      ClassElementList

    ClassElementList:
      ClassElement

    ClassElement:
      MethodDefinition

    MethodDefinition:
      PropertyName ...
      get PropertyName ...
      set PropertyName ...

    PropertyName:
      ComputedPropertyName

    ComputedPropertyName:
      [ AssignmentExpression ]
---*/


let C = class {
  get [1 + 2 - 3 * 4 / 5 ** 6]() {
    return 2.999232;
  }

  set [1 + 2 - 3 * 4 / 5 ** 6](v) {
    return 2.999232;
  }

  static get [1 + 2 - 3 * 4 / 5 ** 6]() {
    return 2.999232;
  }

  static set [1 + 2 - 3 * 4 / 5 ** 6](v) {
    return 2.999232;
  }
};

let c = new C();

assert.sameValue(
  c[1 + 2 - 3 * 4 / 5 ** 6],
  2.999232
);
assert.sameValue(
  c[1 + 2 - 3 * 4 / 5 ** 6] = 2.999232,
  2.999232
);

assert.sameValue(
  C[1 + 2 - 3 * 4 / 5 ** 6],
  2.999232
);
assert.sameValue(
  C[1 + 2 - 3 * 4 / 5 ** 6] = 2.999232,
  2.999232
);
assert.sameValue(
  c[String(1 + 2 - 3 * 4 / 5 ** 6)],
  2.999232
);
assert.sameValue(
  c[String(1 + 2 - 3 * 4 / 5 ** 6)] = 2.999232,
  2.999232
);

assert.sameValue(
  C[String(1 + 2 - 3 * 4 / 5 ** 6)],
  2.999232
);
assert.sameValue(
  C[String(1 + 2 - 3 * 4 / 5 ** 6)] = 2.999232,
  2.999232
);
