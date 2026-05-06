// Copyright (c) 2012 Ecma International.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: |
    Refer 12.6.3; 
    The production 
    IterationStatement : for ( var VariableDeclarationListNoIn ; Expressionopt ; Expressionopt ) Statement
    is evaluated as follows:
es5id: 12.6.3_2-3-a-ii-4
description: >
    The for Statement - (normal, V, empty) will be returned when first
    Expression is a Number object (value is +0)
---*/
// test262 execution-port helpers
var __test262_Object_is = Object.is;
var __test262_Array_isArray = Array.isArray;
var __test262_Object_keys = Object.keys;
var __test262_getOwnPropertyDescriptor = Object.getOwnPropertyDescriptor;
var __test262_defineProperty = Object.defineProperty;
var __test262_hasOwnProperty = Object.prototype.hasOwnProperty;

function __test262_compareArray(actual, expected) {
  if (!__test262_Array_isArray(actual) || !__test262_Array_isArray(expected) || actual.length !== expected.length) {
    return false;
  }
  for (var i = 0; i < actual.length; i++) {
    if (!__test262_Object_is(actual[i], expected[i])) {
      return false;
    }
  }
  return true;
}

function __test262_deepEqual(actual, expected) {
  if (__test262_Object_is(actual, expected)) {
    return true;
  }
  if (__test262_Array_isArray(actual) && __test262_Array_isArray(expected)) {
    if (actual.length !== expected.length) {
      return false;
    }
    for (var i = 0; i < actual.length; i++) {
      if (!__test262_deepEqual(actual[i], expected[i])) {
        return false;
      }
    }
    return true;
  }
  if (actual && expected && typeof actual === "object" && typeof expected === "object") {
    var actualKeys = __test262_Object_keys(actual);
    var expectedKeys = __test262_Object_keys(expected);
    if (!__test262_compareArray(actualKeys, expectedKeys)) {
      return false;
    }
    for (var j = 0; j < actualKeys.length; j++) {
      var key = actualKeys[j];
      if (!__test262_deepEqual(actual[key], expected[key])) {
        return false;
      }
    }
    return true;
  }
  return false;
}

function __test262_isWritable(obj, name, verifyProp, value) {
  var newValue = arguments.length > 3 ? value : "unlikelyValue";
  var verifiedName = verifyProp || name;
  var oldValue = obj[verifiedName];
  var writeSucceeded = false;
  try {
    obj[name] = newValue;
    writeSucceeded = __test262_Object_is(obj[verifiedName], newValue);
  } catch (error) {
    writeSucceeded = false;
  }
  try {
    obj[name] = oldValue;
  } catch (error) {
  }
  return writeSucceeded;
}

function __test262_isEnumerable(obj, name) {
  for (var key in obj) {
    if (key === name) {
      return true;
    }
  }
  return false;
}

function __test262_isConfigurable(obj, name) {
  var descriptor = __test262_getOwnPropertyDescriptor(obj, name);
  if (!descriptor) {
    return false;
  }
  try {
    delete obj[name];
    var deleted = !__test262_hasOwnProperty.call(obj, name);
    __test262_defineProperty(obj, name, descriptor);
    return deleted;
  } catch (error) {
    return false;
  }
}

var assert = function assert(condition) {
  console.log(Boolean(condition));
};

assert.sameValue = function (actual, expected) {
  console.log(__test262_Object_is(actual, expected));
};

assert.notSameValue = function (actual, unexpected) {
  console.log(!__test262_Object_is(actual, unexpected));
};

assert.compareArray = function (actual, expected) {
  console.log(__test262_compareArray(actual, expected));
};

assert.deepEqual = function (actual, expected) {
  console.log(__test262_deepEqual(actual, expected));
};

assert.throws = function (expectedError, fn) {
  var passed = false;
  try {
    fn();
  } catch (error) {
    passed = error instanceof expectedError;
  }
  console.log(passed);
};

function compareArray(actual, expected) {
  return __test262_compareArray(actual, expected);
}

function deepEqual(actual, expected) {
  return __test262_deepEqual(actual, expected);
}

function verifyProperty(obj, name, desc) {
  var actual = __test262_getOwnPropertyDescriptor(obj, name);
  var passed = !!actual;
  if ("value" in desc) {
    passed = passed && __test262_Object_is(actual.value, desc.value);
  }
  if ("writable" in desc) {
    passed = passed && actual.writable === desc.writable;
  }
  if ("enumerable" in desc) {
    passed = passed && actual.enumerable === desc.enumerable;
  }
  if ("configurable" in desc) {
    passed = passed && actual.configurable === desc.configurable;
  }
  console.log(passed);
  return passed;
}

function verifyEqualTo(obj, name, value) {
  var actual = __test262_getOwnPropertyDescriptor(obj, name);
  var passed = !!actual && __test262_Object_is(actual.value, value);
  console.log(passed);
  return passed;
}

function verifyWritable(obj, name, verifyProp, value) {
  var passed = __test262_isWritable(obj, name, verifyProp, value);
  console.log(passed);
  return passed;
}

function verifyNotWritable(obj, name, verifyProp, value) {
  var passed = !__test262_isWritable(obj, name, verifyProp, value);
  console.log(passed);
  return passed;
}

function verifyEnumerable(obj, name) {
  var passed = __test262_isEnumerable(obj, name);
  console.log(passed);
  return passed;
}

function verifyNotEnumerable(obj, name) {
  var passed = !__test262_isEnumerable(obj, name);
  console.log(passed);
  return passed;
}

function verifyConfigurable(obj, name) {
  var passed = __test262_isConfigurable(obj, name);
  console.log(passed);
  return passed;
}

function verifyNotConfigurable(obj, name) {
  var passed = !__test262_isConfigurable(obj, name);
  console.log(passed);
  return passed;
}

function reportCompare() {
}

function Test262Error(message) {
  this.message = message || "";
}
Test262Error.prototype = Object.create(Error.prototype);
Test262Error.prototype.constructor = Test262Error;
Test262Error.prototype.name = "Test262Error";

function print() {
  console.log.apply(console, arguments);
}

function $ERROR() {
  console.log(false);
}

        var accessed = false;
        var numObj = new Number(+0);
        for (var i = 0; numObj;) {
            accessed = true;
            break;
        }

assert(accessed, 'accessed !== true');
