// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: |
    "throw Expression" returns (throw, GetValue(Result(1)), empty), where 1
    evaluates Expression
es5id: 12.13_A2_T3
description: Throwing boolean
---*/

// CHECK#1
try{
  throw true;
}
catch(e){
  console.log(!(e!==true));
}

// CHECK#2
try{
  throw false;
}
catch(e){
  console.log(!(e!==false));
}

// CHECK#3
var b=false;
try{
  throw b;
}
catch(e){
  console.log(!(e!==false));
}

// CHECK#4
var b=true;
try{
  throw b;
}
catch(e){
  console.log(!(e!==true));
}
