"use strict";

// Repro for function-declaration hoisting:
// the function binding must be initialized before any statements run.

module.exports = greet;

greet.answer = 42;

function greet(name) {
    return "Hello, " + name + "!";
}

console.log("typeof module.exports:", typeof module.exports);
console.log("answer:", module.exports.answer);
console.log("result:", module.exports("World"));
