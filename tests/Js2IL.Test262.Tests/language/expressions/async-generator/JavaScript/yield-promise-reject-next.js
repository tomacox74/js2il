// This file was procedurally generated from the following sources:
// - src/async-generators/yield-promise-reject-next.case
// - src/async-generators/default/async-expression.template
/*---
description: yield Promise.reject(value) is treated as throw value (Unnamed async generator expression)
esid: prod-AsyncGeneratorExpression
features: [async-iteration]
flags: [generated, async]
info: |
    Async Generator Function Definitions

    AsyncGeneratorExpression :
      async [no LineTerminator here] function * BindingIdentifier ( FormalParameters ) {
        AsyncGeneratorBody }

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

function __sameValue(actual, expected) {
  return Object.is(actual, expected);
}

function __assertResult(passed, message) {
  console.log(!!passed);
  if (!passed) {
    throw new Error(message || 'Assertion failed');
  }
}

function assert(condition, message) {
  __assertResult(!!condition, message);
}

assert.sameValue = function(actual, expected, message) {
  __assertResult(__sameValue(actual, expected), message || 'Expected SameValue');
};

assert.notSameValue = function(actual, unexpected, message) {
  __assertResult(!__sameValue(actual, unexpected), message || 'Expected values to differ');
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
  __assertResult(passed, message || 'Expected function to throw');
};

assert.compareArray = function(actual, expected, message) {
  var passed = Array.isArray(actual) && Array.isArray(expected) && actual.length === expected.length;
  if (passed) {
    for (var i = 0; i < actual.length; i++) {
      if (!__sameValue(actual[i], expected[i])) {
        passed = false;
        break;
      }
    }
  }
  __assertResult(passed, message || 'Expected arrays to compare equal');
};

function verifyProperty(object, name, desc) {
  var actual = Object.getOwnPropertyDescriptor(object, name);
  var passed = !!actual;

  if (passed && Object.prototype.hasOwnProperty.call(desc, 'value')) {
    passed = __sameValue(actual.value, desc.value);
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

  __assertResult(passed, 'verifyProperty failed for ' + name);
}

let error = new Error();


var callCount = 0;

var gen = async function *() {
  callCount += 1;
  yield Promise.reject(error);
  yield "unreachable";
};

var iter = gen();

iter.next().then(() => {
  throw new Test262Error("Promise incorrectly resolved.");
}, rejectValue => {
  // yield Promise.reject(error);
  assert.sameValue(rejectValue, error);

  iter.next().then(({done, value}) => {
    // iter is closed now.
    assert.sameValue(done, true, "The value of IteratorResult.done is `true`");
    assert.sameValue(value, undefined, "The value of IteratorResult.value is `undefined`");
  }).then($DONE, $DONE);
}).catch($DONE);

assert.sameValue(callCount, 1);
