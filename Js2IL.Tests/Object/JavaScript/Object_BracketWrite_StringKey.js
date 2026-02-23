"use strict";

// Bracket writes with string keys on objects
var obj = { name: "Alice", age: 30 };

obj["name"] = "Bob";
obj["age"] = 25;
obj["city"] = "London";

console.log(obj["name"]);
console.log(obj["age"]);
console.log(obj["city"]);

// Dynamic string key write
var key = "score";
obj[key] = 100;
console.log(obj[key]);

// Bracket write result is the assigned value (assignment expression)
var result = (obj["level"] = 5);
console.log(result);
