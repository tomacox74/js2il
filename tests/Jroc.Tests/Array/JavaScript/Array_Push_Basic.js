"use strict";

var arr = [1, 2];
console.log(arr.push(3)); // length after push -> 3
console.log(arr.length); // 3
console.log(arr.push(4, 5)); // length after pushing two -> 5
for (var i = 0; i < arr.length; i++) { console.log(arr[i]); }
