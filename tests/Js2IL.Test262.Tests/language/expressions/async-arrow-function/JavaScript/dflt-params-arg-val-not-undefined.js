// This file was procedurally generated from the following sources:
// - src/function-forms/dflt-params-arg-val-not-undefined.case
// - src/function-forms/default/async-arrow-function.template
/*---
description: Use of initializer when argument value is not `undefined` (async arrow function expression)
esid: sec-async-arrow-function-definitions
features: [default-parameters, async-functions]
flags: [generated, async]
info: |
    14.7 Async Arrow Function Definitions

    AsyncArrowFunction :
      ...
      CoverCallExpressionAndAsyncArrowHead => AsyncConciseBody

    AsyncConciseBody :
      { AsyncFunctionBody }

    ...

    Supplemental Syntax

    When processing an instance of the production AsyncArrowFunction :
    CoverCallExpressionAndAsyncArrowHead => AsyncConciseBody the interpretation of
    CoverCallExpressionAndAsyncArrowHead is refined using the following grammar:

    AsyncArrowHead :
      async ArrowFormalParameters


    14.1.19 Runtime Semantics: IteratorBindingInitialization

    FormalsList : FormalsList , FormalParameter

    [...]
    23. Let iteratorRecord be Record {[[Iterator]]:
        CreateListIterator(argumentsList), [[Done]]: false}.
    24. If hasDuplicates is true, then
        [...]
    25. Else,
        a. Perform ? IteratorBindingInitialization for formals with
           iteratorRecord and env as arguments.
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

assert.strictEqual = assert.sameValue;
assert.notStrictEqual = assert.notSameValue;

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

var obj = {};
var falseCount = 0;
var stringCount = 0;
var nanCount = 0;
var zeroCount = 0;
var nullCount = 0;
var objCount = 0;


var callCount = 0;

// Stores a reference `ref` for case evaluation
var ref = async (aFalse = falseCount +=1, aString = stringCount += 1, aNaN = nanCount += 1, a0 = zeroCount += 1, aNull = nullCount += 1, aObj = objCount +=1) => {
  assert.sameValue(aFalse, false);
  assert.sameValue(aString, '');
  assert.sameValue(aNaN, NaN);
  assert.sameValue(a0, 0);
  assert.sameValue(aNull, null);
  assert.sameValue(aObj, obj);
  callCount = callCount + 1;
};

ref(false, '', NaN, 0, null, obj).then(() => {
  assert.sameValue(callCount, 1, 'async arrow function invoked exactly once')
}).then($DONE, $DONE);

assert.sameValue(falseCount, 0, 'initializer not evaluated: false');
assert.sameValue(stringCount, 0, 'initializer not evaluated: string');
assert.sameValue(nanCount, 0, 'initializer not evaluated: NaN');
assert.sameValue(zeroCount, 0, 'initializer not evaluated: 0');
assert.sameValue(nullCount, 0, 'initializer not evaluated: null');
assert.sameValue(objCount, 0, 'initializer not evaluated: object');
