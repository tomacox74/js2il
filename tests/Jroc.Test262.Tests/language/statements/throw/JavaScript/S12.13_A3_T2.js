// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: 1. Evaluate Expression
es5id: 12.13_A3_T2
description: Evaluating string expression
---*/

// CHECK#1
try{
  throw "exception"+" #1";
}
catch(e){
  console.log(!(e!=="exception #1"));
}

// CHECK#2
var b="exception"
var a=" #1";
try{
  throw b+a;
}
catch(e){
  console.log(!(e!=="exception #1"));
}
