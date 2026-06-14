// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: Global object properties have attributes { DontEnum }
es5id: 10.2.3_A2.3_T2
description: Global execution context - Function Properties
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

var evalStr =
'//CHECK#1\n'+
'for (var x in this) {\n'+
'  if ( x === \'eval\' ) {\n'+
'    throw new Test262Error("#1: \'eval\' have attribute DontEnum");\n'+
'  } else if ( x === \'parseInt\' ) {\n'+
'    throw new Test262Error("#1: \'parseInt\' have attribute DontEnum");\n'+
'  } else if ( x === \'parseFloat\' ) {\n'+
'    throw new Test262Error("#1: \'parseFloat\' have attribute DontEnum");\n'+
'  } else if ( x === \'isNaN\' ) {\n'+
'    throw new Test262Error("#1: \'isNaN\' have attribute DontEnum");\n'+
'  } else if ( x === \'isFinite\' ) {\n'+
'    throw new Test262Error("#1: \'isFinite\' have attribute DontEnum");\n'+
'  } else if ( x === \'decodeURI\' ) {\n'+
'    throw new Test262Error("#1: \'decodeURI\' have attribute DontEnum");\n'+
'  } else if ( x === \'decodeURIComponent\' ) {\n'+
'    throw new Test262Error("#1: \'decodeURIComponent\' have attribute DontEnum");\n'+
'  } else if ( x === \'encodeURI\' ) {\n'+
'    throw new Test262Error("#1: \'encodeURI\' have attribute DontEnum");\n'+
'  } else if ( x === \'encodeURIComponent\' ) {\n'+
'    throw new Test262Error("#1: \'encodeURIComponent\' have attribute DontEnum");\n'+
'  }\n'+
'}\n';

eval(evalStr);
