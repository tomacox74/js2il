"use strict";

// Bracket reads with string keys on objects
var obj = { name: "Alice", age: 30, score: 99.5 };

console.log(obj["name"]);
console.log(obj["age"]);
console.log(obj["score"]);

// Missing key returns undefined
console.log(obj["missing"]);

// String key on array
var arr = [10, 20, 30];
console.log(arr["length"]);

// Dynamic string key
var key = "name";
console.log(obj[key]);
