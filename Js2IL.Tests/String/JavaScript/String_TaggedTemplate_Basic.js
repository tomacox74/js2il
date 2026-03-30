"use strict";

function tag(strings, ...values) {
    console.log("strings:", strings);
    console.log("values:", values);
    return "result";
}

const x = 5;
const y = 10;
const result = tag`a${x}b${y}c`;
console.log("result:", result);
