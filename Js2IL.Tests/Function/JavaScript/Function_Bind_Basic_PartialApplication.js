"use strict";

function sum3(a, b, c) {
    return a + b + c;
}

const bound = sum3.bind(null, 1, 2);
console.log(bound(3));
