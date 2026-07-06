// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: 1. Evaluate Expression
es5id: 12.13_A3_T1
description: Evaluating boolean expression
---*/

// CHECK#1
var b=true;
try{
  throw b&&false;
}
catch(e){
  console.log(!(e!==false));
}

// CHECK#2
var b=true;
try{
  throw b||false;
}
catch(e){
  console.log(!(e!==true));
}

// CHECK#3
try{
  throw !false;
}
catch(e){
  console.log(!(e!==true));
}

// CHECK#4
var b=true;
try{
  throw !(b&&false);
}
catch(e){
  console.log(!(e!==true));
}
