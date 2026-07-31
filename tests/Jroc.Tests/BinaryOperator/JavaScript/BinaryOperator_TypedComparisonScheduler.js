"use strict";

function neg(a) {
    return -a;
}

function bitNot(a) {
    return ~a;
}

function less(a, b) {
    return a < b;
}

function compareExpr(a, b, c) {
    return a * 2 + 1 <= b - c;
}

function branch(a, b) {
    if (a * 2 < b + 1) {
        return a;
    }
    return b;
}

function boolEq(a, b) {
    return !!a === !!b;
}

var order = "";
function left() {
    order += "L";
    return 1;
}
function right() {
    order += "R";
    return 2;
}

console.log(neg(3));
console.log(Object.is(neg(0), -0));
console.log(bitNot(5));
console.log(less(1, 2), less(NaN, 2), less(Infinity, Infinity));
console.log(compareExpr(2, 10, 3));
console.log(branch(2, 10), branch(10, 2));
console.log(boolEq(1, "x"), boolEq(0, "x"));
console.log(left() < right(), order);

try {
    console.log(tdzValue < 1);
} catch (error) {
    console.log(error instanceof ReferenceError);
}
let tdzValue = 0;
