// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: Global object properties have attributes { DontEnum }
es5id: 10.2.3_A2.1_T3
description: Global execution context - Constructor Properties
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

//CHECK#1
for (var x in this) {
  if (x === 'Object') {
    throw new Test262Error("#1: 'property 'Object' have attribute DontEnum");
  } else if (x === 'Function') {
    throw new Test262Error("#1: 'Function' have attribute DontEnum");
  } else if (x === 'String') {
    throw new Test262Error("#1: 'String' have attribute DontEnum");
  } else if (x === 'Number') {
    throw new Test262Error("#1: 'Number' have attribute DontEnum");
  } else if (x === 'Array') {
    throw new Test262Error("#1: 'Array' have attribute DontEnum");
  } else if (x === 'Boolean') {
    throw new Test262Error("#1: 'Boolean' have attribute DontEnum");
  } else if (x === 'Date') {
    throw new Test262Error("#1: 'Date' have attribute DontEnum");
  } else if (x === 'RegExp') {
    throw new Test262Error("#1: 'RegExp' have attribute DontEnum");
  } else if (x === 'Error') {
    throw new Test262Error("#1: 'Error' have attribute DontEnum");
  } else if (x === 'EvalError') {
    throw new Test262Error("#1: 'EvalError' have attribute DontEnum");
  } else if (x === 'RangeError') {
    throw new Test262Error("#1: 'RangeError' have attribute DontEnum");
  } else if (x === 'ReferenceError') {
    throw new Test262Error("#1: 'ReferenceError' have attribute DontEnum");
  } else if (x === 'SyntaxError') {
    throw new Test262Error("#1: 'SyntaxError' have attribute DontEnum");
  } else if (x === 'TypeError') {
    throw new Test262Error("#1: 'TypeError' have attribute DontEnum");
  } else if (x === 'URIError') {
    throw new Test262Error("#1: 'URIError' have attribute DontEnum");
  }
}
