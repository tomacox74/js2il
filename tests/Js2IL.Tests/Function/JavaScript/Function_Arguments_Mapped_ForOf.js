"use strict";

const mapped = Function(
    "a",
    "b",
    "const values=[];for(const value of arguments){values.push(value);}console.log(values.join(\",\"));arguments[0]=41;console.log(a);");

mapped(1, 2);
