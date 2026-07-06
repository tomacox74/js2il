// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: |
    "throw Expression" returns (throw, GetValue(Result(1)), empty), where 1
    evaluates Expression
es5id: 12.13_A2_T5
description: Throwing number
---*/

// CHECK#1
try{
  throw 13;
}
catch(e){
  console.log(!(e!==13));
}

// CHECK#2
var b=13;
try{
  throw b;
}
catch(e){
  console.log(!(e!==13));
}

// CHECK#3
try{
  throw 2.13;
}
catch(e){
  console.log(!(e!==2.13));
}

// CHECK#4
try{
  throw NaN;
}
catch(e){
  assert.sameValue(e, NaN, "e is NaN");
}

// CHECK#5
try{
  throw +Infinity;
}
catch(e){
  console.log(!(e!==+Infinity));
}

// CHECK#6
try{
  throw -Infinity;
}
catch(e){
  console.log(!(e!==-Infinity));
}

// CHECK#7
try{
  throw +0;
}
catch(e){
  console.log(!(e!==+0));
}

// CHECK#8
try{
  throw -0;
}
catch(e){
  assert.sameValue(e, -0);
}
