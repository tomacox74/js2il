"use strict";

function concat(value) {
    return "value=" + (value * 2);
}

function stringLength(value) {
    return value.length + 1;
}

function arrayLength(value) {
    return value.length + 1;
}

function arrayElement(value) {
    return value[0] + 1;
}

function typedArray(value) {
    return value[0] + value.length;
}

function postfix(value) {
    return [value++ + Number(value), value];
}

var getterCount = 0;
var object = {
    get x() {
        getterCount++;
        return 2;
    }
};

console.log(concat(3));
console.log(stringLength("abc"));
console.log(arrayLength([1, 2, 3]));
console.log(arrayElement([4]));
console.log(typedArray(new Int32Array([5, 6])));
console.log(postfix(3).join(","));
console.log(object.x + object.x, getterCount);

try {
    console.log(tdzValue + 1);
} catch (error) {
    console.log(error instanceof ReferenceError);
}
let tdzValue = 2;
