// This file was procedurally generated from the following sources:
// - src/spread/mult-expr.case
// - src/spread/default/call-expr.template
/*---
description: Spread operator applied to AssignmentExpression following other elements (CallExpression)
esid: sec-function-calls-runtime-semantics-evaluation
flags: [generated]
info: |
    CallExpression : MemberExpression Arguments

    [...]
    9. Return EvaluateDirectCall(func, thisValue, Arguments, tailCall).

    12.3.4.3 Runtime Semantics: EvaluateDirectCall

    1. Let argList be ArgumentListEvaluation(arguments).
    [...]
    6. Let result be Call(func, thisValue, argList).
    [...]

    12.3.6.1 Runtime Semantics: ArgumentListEvaluation

    ArgumentList : ArgumentList , ... AssignmentExpression

    1. Let precedingArgs be the result of evaluating ArgumentList.
    2. Let spreadRef be the result of evaluating AssignmentExpression.
    3. Let iterator be GetIterator(GetValue(spreadRef) ).
    4. ReturnIfAbrupt(iterator).
    5. Repeat
       a. Let next be IteratorStep(iterator).
       b. ReturnIfAbrupt(next).
       c. If next is false, return precedingArgs.
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

var source = [3, 4, 5];
var target;

var callCount = 0;

(function() {
  assert.sameValue(arguments.length, 5);
  assert.sameValue(arguments[0], 1);
  assert.sameValue(arguments[1], 2);
  assert.sameValue(arguments[2], 3);
  assert.sameValue(arguments[3], 4);
  assert.sameValue(arguments[4], 5);
  assert.sameValue(target, source);
  callCount += 1;
}(1, 2, ...target = source));

assert.sameValue(callCount, 1);
