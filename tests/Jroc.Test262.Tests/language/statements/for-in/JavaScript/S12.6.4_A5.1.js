// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: |
    The production IterationStatement: "for (var VariableDeclarationNoIn in
    Expression) Statement"
es5id: 12.6.4_A5.1
description: >
    Using hierarchical Object as an Expression is appropriate. The
    depth is two
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
    return message || fallback || "Assertion failed";
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
        throw new Error(__test262FormatMessage(message, "Expected SameValue"));
    }
};

assert.notSameValue = function(actual, unexpected, message) {
    var passed = !__test262SameValue(actual, unexpected);
    console.log(passed);
    if (!passed) {
        throw new Error(__test262FormatMessage(message, "Expected values to differ"));
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
        throw new Error(__test262FormatMessage(message, "Expected function to throw"));
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
        throw new Error(__test262FormatMessage(message, "Expected arrays to match"));
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
var __hash__map, __arr;

__hash__map={a:{aa:1,ab:2,ac:3,ad:4},b:{ba:1,bb:2,bc:3,bd:4},c:{ca:1,cb:2,cc:3,cd:4},d:{da:1,db:2,dc:3,dd:4}};

__arr = "";

for(var __key in __hash__map){
    for (var __ind in __hash__map[__key]){
        __arr+=("" + __ind + __hash__map[__key][__ind]);
    }
}

if(!(
(__arr.indexOf("aa1")!==-1)&
(__arr.indexOf("ab2")!==-1)&
(__arr.indexOf("ac3")!==-1)&
(__arr.indexOf("ad4")!==-1)&
(__arr.indexOf("ba1")!==-1)&
(__arr.indexOf("bb2")!==-1)&
(__arr.indexOf("bc3")!==-1)&
(__arr.indexOf("bd4")!==-1)&
(__arr.indexOf("ca1")!==-1)&
(__arr.indexOf("cb2")!==-1)&
(__arr.indexOf("cc3")!==-1)&
(__arr.indexOf("cd4")!==-1)&
(__arr.indexOf("da1")!==-1)&
(__arr.indexOf("db2")!==-1)&
(__arr.indexOf("dc3")!==-1)&
(__arr.indexOf("dd4")!==-1)
)) throw new Test262Error('#1: The nested for-in Statement applied to hierarchial object works properly as described in the Standard');

console.log(true);
