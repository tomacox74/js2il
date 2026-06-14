function Test262Error(message) {
    this.message = message || "";
    this.name = "Test262Error";
}
Test262Error.prototype = Object.create(Error.prototype);
Test262Error.prototype.constructor = Test262Error;

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

function verifyProperty(object, name, expected) {
    var desc = Object.getOwnPropertyDescriptor(object, name);
    if ('value' in expected) {
        assert.sameValue(desc.value, expected.value, 'descriptor value');
    }
    if ('writable' in expected) {
        assert.sameValue(desc.writable, expected.writable, 'descriptor writable');
    }
    if ('enumerable' in expected) {
        assert.sameValue(desc.enumerable, expected.enumerable, 'descriptor enumerable');
    }
    if ('configurable' in expected) {
        assert.sameValue(desc.configurable, expected.configurable, 'descriptor configurable');
    }
}
// This file was procedurally generated from the following sources:
// - src/computed-property-names/computed-property-name-from-expression-logical-and.case
// - src/computed-property-names/evaluation/class-declaration-accessors.template
/*---
description: Computed property name from logical and (ComputedPropertyName in ClassDeclaration)
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
let x = 0;


class C {
  get [x && 1]() {
    return 2;
  }

  set [x && 1](v) {
    return 2;
  }

  static get [x && 1]() {
    return 2;
  }

  static set [x && 1](v) {
    return 2;
  }
};

let c = new C();

assert.sameValue(
  c[x && 1],
  2
);
assert.sameValue(
  c[x && 1] = 2,
  2
);

assert.sameValue(
  C[x && 1],
  2
);
assert.sameValue(
  C[x && 1] = 2,
  2
);
assert.sameValue(
  c[String(x && 1)],
  2
);
assert.sameValue(
  c[String(x && 1)] = 2,
  2
);

assert.sameValue(
  C[String(x && 1)],
  2
);
assert.sameValue(
  C[String(x && 1)] = 2,
  2
);

assert.sameValue(x, 0);
