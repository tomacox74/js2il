// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: Catching objects with try/catch/finally statement
es5id: 12.14_A18_T4
description: Catching string
---*/


function assert(value) {
  console.log(!!value);
}

assert.sameValue = function(actual, expected) {
  console.log(Object.is(actual, expected));
};

assert.notSameValue = function(actual, unexpected) {
  console.log(!Object.is(actual, unexpected));
};

assert.compareArray = function(actual, expected) {
  if (!Array.isArray(actual) || !Array.isArray(expected) || actual.length !== expected.length) {
    console.log(false);
    return;
  }

  for (var i = 0; i < actual.length; i++) {
    if (!Object.is(actual[i], expected[i])) {
      console.log(false);
      return;
    }
  }

  console.log(true);
};

assert.throws = function(expectedCtor, fn) {
  try {
    fn();
    console.log(false);
  } catch (error) {
    console.log(error instanceof expectedCtor);
  }
};

function Test262Error(message) {
  this.name = 'Test262Error';
  this.message = message || '';
}

Test262Error.prototype = Object.create(Error.prototype);
Test262Error.prototype.constructor = Test262Error;

// CHECK#1
try{
  throw "exception #1";
}
catch(e){
  if (e!=="exception #1") throw new Test262Error('#1: Exception ==="exception #1". Actual:  Exception ==='+ e  );
}

// CHECK#2
try{
  throw "exception"+" #1";
}
catch(e){
  if (e!=="exception #1") throw new Test262Error('#2: Exception ==="exception #1". Actual:  Exception ==='+ e  );
}

// CHECK#3
var b="exception #1";
try{
  throw b;
}
catch(e){
  if (e!=="exception #1") throw new Test262Error('#3: Exception ==="exception #1". Actual:  Exception ==='+ e  );
}

// CHECK#4
var a="exception";
var b=" #1";
try{
  throw a+b;
}
catch(e){
  if (e!=="exception #1") throw new Test262Error('#4: Exception ==="exception #1". Actual:  Exception ==='+ e  );
}


console.log("pass");
