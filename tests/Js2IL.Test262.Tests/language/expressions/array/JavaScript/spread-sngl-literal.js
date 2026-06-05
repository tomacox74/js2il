// This file was procedurally generated from the following sources:
// - src/spread/sngl-literal.case
// - src/spread/default/array.template
/*---
description: Spread operator applied to array literal as only element (Array initializer)
esid: sec-runtime-semantics-arrayaccumulation
flags: [generated]
info: |
    SpreadElement : ...AssignmentExpression

    1. Let spreadRef be the result of evaluating AssignmentExpression.
    2. Let spreadObj be ? GetValue(spreadRef).
    3. Let iterator be ? GetIterator(spreadObj).
    4. Repeat
       a. Let next be ? IteratorStep(iterator).
       b. If next is false, return nextIndex.
       c. Let nextValue be ? IteratorValue(next).
       d. Let status be CreateDataProperty(array, ToString(ToUint32(nextIndex)),
          nextValue).
       e. Assert: status is true.
       f. Let nextIndex be nextIndex + 1.

    12.3.6.1 Runtime Semantics: ArgumentListEvaluation

    ArgumentList : ... AssignmentExpression

    1. Let list be an empty List.
    2. Let spreadRef be the result of evaluating AssignmentExpression.
    3. Let spreadObj be GetValue(spreadRef).
    4. Let iterator be GetIterator(spreadObj).
    5. ReturnIfAbrupt(iterator).
    6. Repeat
       a. Let next be IteratorStep(iterator).
       b. ReturnIfAbrupt(next).
       c. If next is false, return list.
       d. Let nextArg be IteratorValue(next).
       e. ReturnIfAbrupt(nextArg).
       f. Append nextArg as the last element of list.
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

var callCount = 0;

(function() {
  assert.sameValue(arguments.length, 3);
  assert.sameValue(arguments[0], 3);
  assert.sameValue(arguments[1], 4);
  assert.sameValue(arguments[2], 5);
  callCount += 1;
}.apply(null, [...[3, 4, 5]]));

assert.sameValue(callCount, 1);
