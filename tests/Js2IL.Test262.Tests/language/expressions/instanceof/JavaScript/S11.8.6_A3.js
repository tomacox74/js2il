// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: If ShiftExpression is not an object, throw TypeError
es5id: 11.8.6_A3
description: Checking all the types of primitives
---*/

//CHECK#1
let __result1 = false;
try {
  true instanceof true;
} catch (e) {
  __result1 = !(e instanceof TypeError !== true);
}
console.log(__result1);

//CHECK#2
let __result2 = false;
try {
  1 instanceof 1;
} catch (e) {
  __result2 = !(e instanceof TypeError !== true);
}
console.log(__result2);

//CHECK#3
let __result3 = false;
try {
  "string" instanceof "string";
} catch (e) {
  __result3 = !(e instanceof TypeError !== true);
}
console.log(__result3);

//CHECK#4
let __result4 = false;
try {
  undefined instanceof undefined;
} catch (e) {
  __result4 = !(e instanceof TypeError !== true);
}
console.log(__result4);

//CHECK#5
let __result5 = false;
try {
  null instanceof null;
} catch (e) {
  __result5 = !(e instanceof TypeError !== true);
}
console.log(__result5);

