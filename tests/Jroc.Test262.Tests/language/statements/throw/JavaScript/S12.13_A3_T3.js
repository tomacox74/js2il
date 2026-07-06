// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: 1. Evaluate Expression
es5id: 12.13_A3_T3
description: Evaluating number expression
---*/

// CHECK#1
try{
  throw 10+3;
}
catch(e){
  console.log(!(e!==13));
}

// CHECK#2
var b=10;
var a=3;
try{
  throw a+b;
}
catch(e){
  console.log(!(e!==13));
}

// CHECK#3
try{
  throw 3.15-1.02;
}
catch(e){
  console.log(!(e!==2.13));
}

// CHECK#4
try{
  throw 2*2;
}
catch(e){
  console.log(!(e!==4));
}

// CHECK#5
try{
  throw 1+Infinity;
}
catch(e){
  console.log(!(e!==+Infinity));
}

// CHECK#6
try{
  throw 1-Infinity;
}
catch(e){
  console.log(!(e!==-Infinity));
}

// CHECK#7
try{
  throw 10/5;
}
catch(e){
  console.log(!(e!==2));
}

// CHECK#8
try{
  throw 8>>2;
}
catch(e){
  console.log(!(e!==2));
}

// CHECK#9
try{
  throw 2<<2;
}
catch(e){
  console.log(!(e!==8));
}

// CHECK#10
try{
  throw 123%100;
}
catch(e){
  console.log(!(e!==23));
}
