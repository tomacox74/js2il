"use strict";

function tag(strings) {
    console.log("strings.length:", strings.length);
    console.log("strings[0]:", strings[0]);
    return "done";
}

const result = tag`hello world`;
console.log("result:", result);
