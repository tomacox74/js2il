"use strict";

const mapped = Function(
    "a",
    "b",
    "console.log(arguments.length);console.log(delete arguments.length);console.log('length' in arguments);console.log(arguments.length);console.log(Object.getOwnPropertyNames(arguments).join(','));");

mapped(1, 2);

const getArgs = Function("return arguments;");
const escaped = getArgs(4, 5);
console.log(escaped.length);
console.log(delete escaped.length);
console.log("length" in escaped);
console.log(escaped.length);
console.log(Object.getOwnPropertyNames(escaped).join(","));
