// This file was procedurally generated from the following sources:
// - src/dstr-assignment/array-rest-lref.case
// - src/dstr-assignment/default/assignment-expr.template
/*---
description: Reference is evaluated during assignment (AssignmentExpression)
esid: sec-variable-statement-runtime-semantics-evaluation
features: [Symbol.iterator, destructuring-binding]
flags: [generated]
info: |
    VariableDeclaration : BindingPattern Initializer

    1. Let rhs be the result of evaluating Initializer.
    2. Let rval be GetValue(rhs).
    3. ReturnIfAbrupt(rval).
    4. Return the result of performing BindingInitialization for
       BindingPattern passing rval and undefined as arguments.

    ArrayAssignmentPattern : [ Elisionopt AssignmentRestElement ]

    [...]
    5. Let result be the result of performing
       IteratorDestructuringAssignmentEvaluation of AssignmentRestElement with
       iteratorRecord as the argument
    6. If iteratorRecord.[[done]] is false, return IteratorClose(iterator,
       result).

    AssignmentRestElement[Yield] : ... DestructuringAssignmentTarget

    1. If DestructuringAssignmentTarget is neither an ObjectLiteral nor an
       ArrayLiteral, then
       a. Let lref be the result of evaluating DestructuringAssignmentTarget.
       b. ReturnIfAbrupt(lref).
    [...]

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

var nextCount = 0;
var returnCount = 0;
var iterable = {};
var iterator = {
  next: function() {
    nextCount += 1;
    return { done: true };
  },
  return: function() {
    returnCount += 1;
  }
};
var obj = {};
iterable[Symbol.iterator] = function() {
  return iterator;
};

var result;
var vals = iterable;

result = [...obj['a' + 'b']] = vals;

assert.sameValue(nextCount, 1);
assert.sameValue(returnCount, 0);
assert(!!obj.ab);
assert.sameValue(obj.ab.length, 0);

assert.sameValue(result, vals);
