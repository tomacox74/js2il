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
// - src/computed-property-names/computed-property-name-from-integer-e-notational-literal.case
// - src/computed-property-names/evaluation/class-expression.template
/*---
description: Computed property name from numeric literal (ComputedPropertyName in ClassExpression)
esid: prod-ComputedPropertyName
features: [computed-property-names]
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
  [1]() {
    return 2;
  }
  static [1]() {
    return 2;
  }
};

let c = new C();

assert.sameValue(
  c[1](),
  2
);
assert.sameValue(
  C[1](),
  2
);
assert.sameValue(
  c[String(1)](),
  2
);
assert.sameValue(
  C[String(1)](),
  2
);
