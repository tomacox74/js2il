"use strict";

function tag(strings, ...values) {
    console.log("tag called");
    return "result";
}

let counter = 0;
function getValue() {
    console.log("getValue called:", counter);
    return counter++;
}

const result = tag`a${getValue()}b${getValue()}c`;
console.log("final result:", result);
