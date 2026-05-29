// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: Catching objects with try/catch/finally statement
es5id: 12.14_A18_T7
description: Catching Array
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

var mycars = new Array();
mycars[0] = "Saab";
mycars[1] = "Volvo";
mycars[2] = "BMW";

var mycars2 = new Array();
mycars2[0] = "Mercedes";
mycars2[1] = "Jeep";
mycars2[2] = "Suzuki";

// CHECK#1
try{
  throw mycars;
}
catch(e){
  for (var i=0;i<3;i++){
    if (e[i]!==mycars[i]) throw new Test262Error('#1.'+i+': Exception['+i+']===mycars['+i+']. Actual:  Exception['+i+']==='+ e[i] );
  }
}

// CHECK#2
try{
  throw mycars.concat(mycars2);
}
catch(e){
  for (var i=0;i<3;i++){
    if (e[i]!==mycars[i]) throw new Test262Error('#2.'+i+': Exception['+i+']===mycars['+i+']. Actual:  Exception['+i+']==='+ e[i] );
  }
  for (var i=3;i<6;i++){
    if (e[i]!==mycars2[i-3]) throw new Test262Error('#2.'+i+': Exception['+i+']===mycars2['+i+']. Actual:  Exception['+i+']==='+ e[i] );
  }
}

// CHECK#3
try{
  throw new Array("Mercedes","Jeep","Suzuki");
}
catch(e){
  for (var i=0;i<3;i++){
    if (e[i]!==mycars2[i]) throw new Test262Error('#3.'+i+': Exception['+i+']===mycars2['+i+']. Actual:  Exception['+i+']==='+ e[i]);
  }
}

// CHECK#4
try{
  throw mycars.concat(new Array("Mercedes","Jeep","Suzuki"));
}
catch(e){
  for (var i=0;i<3;i++){
    if (e[i]!==mycars[i]) throw new Test262Error('#4.'+i+': Exception['+i+']===mycars['+i+']. Actual:  Exception['+i+']==='+ e[i] );
  }
  for (var i=3;i<6;i++){
    if (e[i]!==mycars2[i-3]) throw new Test262Error('#4.'+i+': Exception['+i+']===mycars2['+(i-3)+']. Actual:  Exception['+i+']==='+ e[i]);
  }
}


console.log("pass");
