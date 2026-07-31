"use strict";

function mulAdd(factor) {
    return factor * 2 + 1;
}

function addChain(a, b, c, d) {
    return a + b + c + d;
}

function tree(a, b, c, d) {
    return a * b + c * d;
}

function subChain(a, b, c) {
    return a - b - c;
}

function divMod(a, b, c) {
    return a / b % c;
}

function expAdd(a, b, c) {
    return a ** b + c;
}

var order = "";
function left() {
    order += "L";
    return 2;
}
function right() {
    order += "R";
    return 3;
}
function calls() {
    return left() + right();
}

function postfix(x) {
    return [x++ + x, x];
}

console.log(mulAdd(4));
console.log(addChain(1, 2, 3, 4));
console.log(tree(2, 3, 4, 5));
console.log(subChain(10, 3, 2));
console.log(divMod(20, 3, 2));
console.log(expAdd(2, 3, 1));
console.log(Number.isNaN(mulAdd(NaN)));
console.log(Object.is(divMod(-0, 1, 2), -0));
console.log(calls(), order);
console.log(postfix(3).join(","));
