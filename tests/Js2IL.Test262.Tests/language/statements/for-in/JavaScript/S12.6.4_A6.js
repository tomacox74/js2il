// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: |
    The production IterationStatement: "for (var VariableDeclarationNoIn in
    Expression) Statement"
es5id: 12.6.4_A6
description: >
    Using Object with custom prototype as an Expression is
    appropriate. The prototype is "{feat:2,hint:"protohint"}"
---*/

function Test262Error(message) {
  this.name = 'Test262Error';
  this.message = message === undefined ? '' : String(message);
}
function __test262SameValue(a, b) {
  return Object.is(a, b);
}
function compareArray(actual, expected) {
  if (!actual || !expected || actual.length !== expected.length) {
    return false;
  }
  for (let i = 0; i < actual.length; i++) {
    if (!Object.is(actual[i], expected[i])) {
      return false;
    }
  }
  return true;
}
function verifyProperty(obj, name, desc) {
  const actual = Object.getOwnPropertyDescriptor(obj, name);
  let ok = !!actual;
  if ('value' in desc) ok = ok && Object.is(actual.value, desc.value);
  if ('writable' in desc) ok = ok && actual.writable === desc.writable;
  if ('enumerable' in desc) ok = ok && actual.enumerable === desc.enumerable;
  if ('configurable' in desc) ok = ok && actual.configurable === desc.configurable;
  if ('get' in desc) ok = ok && actual.get === desc.get;
  if ('set' in desc) ok = ok && actual.set === desc.set;
  console.log(ok);
  return ok;
}
var assert = function assert(condition) {
  console.log(!!condition);
};
assert.sameValue = function(actual, expected) {
  console.log(__test262SameValue(actual, expected));
};
assert.notSameValue = function(actual, unexpected) {
  console.log(!__test262SameValue(actual, unexpected));
};
assert.compareArray = function(actual, expected) {
  console.log(compareArray(actual, expected));
};
assert.throws = function(ExpectedError, fn) {
  try {
    fn();
    console.log(false);
  } catch (error) {
    console.log(error instanceof ExpectedError || error.constructor === ExpectedError || error.name === ExpectedError.name);
  }
};

var __accum, key;

function FACTORY(){this.prop=1;this.hint="hinted"};

FACTORY.prototype = {feat:2,hint:"protohint"};

var __instance = new FACTORY;

__accum="";

for (key in __instance){
	__accum+=(key + __instance[key]);
}

//////////////////////////////////////////////////////////////////////////////
//CHECK#1
if (!((__accum.indexOf("prop1")!==-1)&&(__accum.indexOf("feat2")!==-1)&&(__accum.indexOf("hinthinted")!==-1))) {
	throw new Test262Error('#1: (__accum.indexOf("prop1")!==-1)&&(__accum.indexOf("feat2")!==-1)&&(__accum.indexOf("hinthinted")!==-1)');
}
//
//////////////////////////////////////////////////////////////////////////////

//////////////////////////////////////////////////////////////////////////////
//CHECK#2
if (__accum.indexOf("hintprotohint")!==-1) {
	throw new Test262Error('#2: __accum.indexOf("hintprotohint") === -1. Actual:  __accum.indexOf("hintprotohint") ==='+ __accum.indexOf("hintprotohint")  );
}
//
//////////////////////////////////////////////////////////////////////////////

