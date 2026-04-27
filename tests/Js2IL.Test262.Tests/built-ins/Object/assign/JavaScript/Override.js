// Copyright 2015 Microsoft Corporation. All rights reserved.
// This code is governed by the license found in the LICENSE file.

/*---
description: Test Object.Assign(target,...sources).
esid: sec-object.assign
---*/

//"a" will be an property of the final object and the value should be 1
var target = {
  a: 1
};
/*
"1a2c3" have own enumerable properties, so it Should be wrapped to objects;
{b:6} is an object,should be assigned to final object.
undefined and null should be ignored;
125 is a number,it cannot has own enumerable properties;
{a:"c"},{a:5} will override property a, the value should be 5.
*/
var result = Object.assign(target, "1a2c3", {
  a: "c"
}, undefined, {
  b: 6
}, null, 125, {
  a: 5
});

console.log(Object.is(Object.getOwnPropertyNames(result).length, 7));
console.log(Object.is(result.a, 5));
console.log(Object.is(result[0], "1"));
console.log(Object.is(result[1], "a"));
console.log(Object.is(result[2], "2"));
console.log(Object.is(result[3], "c"));
console.log(Object.is(result[4], "3"));
console.log(Object.is(result.b, 6));
