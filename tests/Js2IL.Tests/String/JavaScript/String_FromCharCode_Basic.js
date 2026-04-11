"use strict";

var empty = String.fromCharCode();
console.log(empty.length);

console.log(String.fromCharCode(65));
console.log(String.fromCharCode(65, 66, 67));
console.log(String.fromCharCode(65536 + 65));
console.log(String.fromCharCode("66"));
console.log(String.fromCharCode(NaN).charCodeAt(0));
console.log(String.fromCharCode(-1).charCodeAt(0));
