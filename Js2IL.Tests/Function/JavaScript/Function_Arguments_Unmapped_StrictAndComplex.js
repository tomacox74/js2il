"use strict";

const strictFn = Function(
    "a",
    "\"use strict\";console.log(a);console.log(arguments[0]);arguments[0]=41;console.log(a);console.log(arguments[0]);a=99;console.log(a);console.log(arguments[0]);");

strictFn(1);

const complexFn = Function(
    "a = 10",
    "console.log(a);console.log(arguments[0]);arguments[0]=41;console.log(a);console.log(arguments[0]);a=99;console.log(a);console.log(arguments[0]);console.log(Object.keys(arguments).join(\",\"));");

complexFn(1);
