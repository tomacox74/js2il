// Copyright (C) 2016 the V8 project authors. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.
/*---
esid: sec-functiondeclarationinstantiation
description: >
    Creation of new lexical environment (distinct from the variable
    environment) for the function body outside of strict mode
info: |
    [...]
    29. If strict is false, then
        a. Let lexEnv be NewDeclarativeEnvironment(varEnv).
        b. NOTE: Non-strict functions use a separate lexical Environment Record
           for top-level lexical declarations so that a direct eval can
           determine whether any var scoped declarations introduced by the eval
           code conflict with pre-existing top-level lexically scoped
           declarations.  This is not needed for strict functions because a
           strict direct eval always places all declarations into a new
           Environment Record.
    [...]

    18.2.1.3 Runtime Semantics: EvalDeclarationInstantiation

    [...]
    5. If strict is false, then
       [...]
       b. Let thisLex be lexEnv.
       c. Assert: The following loop will terminate.
       d. Repeat while thisLex is not the same as varEnv,
          i. Let thisEnvRec be thisLex's EnvironmentRecord.
          ii. If thisEnvRec is not an object Environment Record, then
              1. NOTE: The environment of with statements cannot contain any
                 lexical declaration so it doesn't need to be checked for
                 var/let hoisting conflicts.
              2. For each name in varNames, do
                 a. If thisEnvRec.HasBinding(name) is true, then
                    i. Throw a SyntaxError exception.
                    ii. NOTE: Annex B.3.5 defines alternate semantics for the
                        above step.
                 b. NOTE: A direct eval will not hoist var declaration over a
                    like-named lexical declaration.
          iii. Let thisLex be thisLex's outer environment reference.
flags: [noStrict]
features: [let]
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

assert.deepEqual = function (actual, expected) {
  console.log(__test262_deepEqual(actual, expected));
};

function deepEqual(actual, expected) {
  return __test262_deepEqual(actual, expected);
}

function verifyEqualTo(obj, name, value) {
  var actual = __test262_getOwnPropertyDescriptor(obj, name);
  var passed = !!actual && __test262_Object_is(actual.value, value);
  console.log(passed);
  return passed;
}

function reportCompare() {
}

Test262Error.prototype.name = "Test262Error";

function print() {
  console.log.apply(console, arguments);
}

assert.throws(SyntaxError, function() {
  let x;
  eval('var x;');
});
