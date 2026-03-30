"use strict";

function format(prefix, suffix, ...middle) {
    let result = prefix;
    for (let i = 0; i < middle.length; i++) {
        result = result + middle[i];
    }
    result = result + suffix;
    return result;
}

console.log(format("[", "]", "a", "b", "c"));
console.log(format("<", ">", "hello"));
console.log(format("{", "}"));
