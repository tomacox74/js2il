"use strict";

function add(a, b) {
    return a + b;
}

console.log(add.apply(null, [1, 2]));
