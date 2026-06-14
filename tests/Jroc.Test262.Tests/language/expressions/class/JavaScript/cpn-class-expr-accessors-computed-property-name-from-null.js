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
// This file was procedurally generated from the following sources:
// - src/computed-property-names/computed-property-name-from-null.case
// - src/computed-property-names/evaluation/class-expression-accessors.template
/*---
description: Computed property name from condition expression (ComputedPropertyName in ClassExpression)
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
  get [null]() {
    return null;
  }

  set [null](v) {
    return null;
  }

  static get [null]() {
    return null;
  }

  static set [null](v) {
    return null;
  }
};

let c = new C();

assert.sameValue(
  c[null],
  null
);
assert.sameValue(
  c[null] = null,
  null
);

assert.sameValue(
  C[null],
  null
);
assert.sameValue(
  C[null] = null,
  null
);
assert.sameValue(
  c[String(null)],
  null
);
assert.sameValue(
  c[String(null)] = null,
  null
);

assert.sameValue(
  C[String(null)],
  null
);
assert.sameValue(
  C[String(null)] = null,
  null
);
