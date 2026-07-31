"use strict";

function args(a, b) {
    return Math.max(a * 2, b + 3);
}

function arrayValue(a, b) {
    return [a * 2, b + 3];
}

function objectValue(a, b) {
    return { x: a * 2, y: b + 3 };
}

function nested(a, b) {
    return [[a + 1], { y: b * 2 }];
}

function deepArray(a, b, c, d, e, f, g, h, i) {
    return [a + (b + (c + (d + (e + (f + (g + (h + i)))))))];
}

let order = "";
function f(value) {
    order += value;
    return value;
}

function arrayOrder() {
    order = "";
    const result = [[f("a")], f("b"), order];
    return result[0][0] + result[1] + ":" + result[2] + ":" + order;
}

function objectOrder() {
    order = "";
    const result = { [f("k")]: f("v"), order };
    return result.k + ":" + result.order + ":" + order;
}

const spread = {
    [Symbol.iterator]() {
        order += "i";
        return [1][Symbol.iterator]();
    }
};

function spreadOrder() {
    order = "";
    const result = [f("a"), ...spread, f("b")];
    return result.join(",") + ":" + order;
}

function exceptionOrder() {
    order = "";
    try {
        return [throwValue("a"), f("b")];
    } catch (error) {
        return order;
    }
}

function throwValue(value) {
    order += value;
    throw new Error(value);
}

console.log(args(2, 4));
console.log(arrayValue(2, 4).join(","));
const objectResult = objectValue(2, 4);
console.log(objectResult.x + "," + objectResult.y);
const nestedResult = nested(2, 4);
console.log(nestedResult[0][0] + "," + nestedResult[1].y);
console.log(deepArray(1, 2, 3, 4, 5, 6, 7, 8, 9)[0]);
console.log(arrayOrder());
console.log(objectOrder());
console.log(spreadOrder());
console.log(exceptionOrder());
