"use strict";

// Tests that Number() called on a string (object-stored) is NOT CSE'd:
// each call is independent. This verifies we don't mistakenly CSE
// coercions whose source is a reference type rather than a primitive.

let s = "42";
let a = Number(s); // string -> number conversion
let b = Number(s); // same string, same result
console.log(a);    // 42
console.log(b);    // 42
console.log(a === b); // true
