// This file was procedurally generated from the following sources:
// - src/dstr-assignment/array-elision-val-symbol.case
// - src/dstr-assignment/error/assignment-expr.template
/*---
description: An ArrayAssignmentPattern containing only Elisions requires iterable values and throws for symbol values. (AssignmentExpression)
esid: sec-variable-statement-runtime-semantics-evaluation
features: [Symbol, destructuring-binding]
flags: [generated]
info: |
    VariableDeclaration : BindingPattern Initializer

    1. Let rhs be the result of evaluating Initializer.
    2. Let rval be GetValue(rhs).
    3. ReturnIfAbrupt(rval).
    4. Return the result of performing BindingInitialization for
       BindingPattern passing rval and undefined as arguments.
---*/

function Test262Error(message) {
  this.name = 'Test262Error';
  this.message = message || '';
}

Test262Error.prototype = Object.create(Error.prototype);
Test262Error.prototype.constructor = Test262Error;

function $ERROR(message) {
  throw new Test262Error(message);
}

function $DONE(error) {
  if (error) {
    throw error;
  }
}

function __test262SameValue(actual, expected) {
  return Object.is(actual, expected);
}

function __test262FormatMessage(message, fallback) {
  return message || fallback || 'Assertion failed';
}

function assert(condition, message) {
  var passed = !!condition;
  console.log(passed);
  if (!passed) {
    throw new Error(__test262FormatMessage(message));
  }
}

assert.sameValue = function(actual, expected, message) {
  var passed = __test262SameValue(actual, expected);
  console.log(passed);
  if (!passed) {
    throw new Error(__test262FormatMessage(message, 'Expected SameValue'));
  }
};

assert.notSameValue = function(actual, unexpected, message) {
  var passed = !__test262SameValue(actual, unexpected);
  console.log(passed);
  if (!passed) {
    throw new Error(__test262FormatMessage(message, 'Expected values to differ'));
  }
};

function compareArray(actual, expected) {
  if (!actual || !expected || actual.length !== expected.length) {
    return false;
  }

  for (var i = 0; i < actual.length; i++) {
    if (!__test262SameValue(actual[i], expected[i])) {
      return false;
    }
  }

  return true;
}

assert.compareArray = function(actual, expected, message) {
  var passed = compareArray(actual, expected);
  console.log(passed);
  if (!passed) {
    throw new Error(__test262FormatMessage(message, 'Expected arrays to match'));
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
    throw new Error(__test262FormatMessage(message, 'Expected function to throw'));
  }
};

function verifyProperty(object, name, desc) {
  var actual = Object.getOwnPropertyDescriptor(object, name);
  var passed = !!actual;

  if (passed && Object.prototype.hasOwnProperty.call(desc, 'value')) {
    passed = __test262SameValue(actual.value, desc.value);
  }

  if (passed && Object.prototype.hasOwnProperty.call(desc, 'writable')) {
    passed = actual.writable === desc.writable;
  }

  if (passed && Object.prototype.hasOwnProperty.call(desc, 'enumerable')) {
    passed = actual.enumerable === desc.enumerable;
  }

  if (passed && Object.prototype.hasOwnProperty.call(desc, 'configurable')) {
    passed = actual.configurable === desc.configurable;
  }

  if (passed && Object.prototype.hasOwnProperty.call(desc, 'get')) {
    passed = actual.get === desc.get;
  }

  if (passed && Object.prototype.hasOwnProperty.call(desc, 'set')) {
    passed = actual.set === desc.set;
  }

  console.log(passed);
  if (!passed) {
    throw new Error('verifyProperty failed for ' + name);
  }
}

var s = Symbol();

assert.throws(TypeError, function() {
  0, [,] = s;
});
