// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: |
    Properties of the object being enumerated may be deleted during
    enumeration
es5id: 12.6.4_A7_T1
description: >
    Checking "for (LeftHandSideExpression in Expression) Statement"
    case
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

var __obj, __accum, __key;

__obj={aa:1,ba:2,ca:3};

__accum="";

for (__key in __obj){
	
    erasator_T_1000(__obj,"b");
  
	__accum+=(__key+__obj[__key]);
	
}


//////////////////////////////////////////////////////////////////////////////
//CHECK#1
if (!((__accum.indexOf("aa1")!==-1)&&(__accum.indexOf("ca3")!==-1))) {
	throw new Test262Error('#1: (__accum.indexOf("aa1")!==-1)&&(__accum.indexOf("ca3")!==-1)');
}
//
//////////////////////////////////////////////////////////////////////////////

//////////////////////////////////////////////////////////////////////////////
//CHECK#2
if (__accum.indexOf("ba2")!==-1) {
	throw new Test262Error('#2: __accum.indexOf("ba2") === -1. Actual:  __accum.indexOf("ba2") ==='+ __accum.indexOf("ba2")  );
}
//
//////////////////////////////////////////////////////////////////////////////


// erasator is the hash map terminator
function erasator_T_1000(hash_map, charactr){
	for (var key in hash_map){
		if (key.indexOf(charactr)===0) {
			delete hash_map[key];
		};
	}
}

