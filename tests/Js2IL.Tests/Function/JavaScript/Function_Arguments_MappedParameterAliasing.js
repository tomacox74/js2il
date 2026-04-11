"use strict";

const mapped = Function(
    "a",
    "b",
    "console.log(arguments.length);console.log(a);console.log(arguments[0]);arguments[0]=41;console.log(a);console.log(arguments[0]);a=99;console.log(a);console.log(arguments[0]);console.log(b);arguments[1]=17;console.log(b);console.log(Object.keys(arguments).join(\",\"));");

mapped(1, 2);
