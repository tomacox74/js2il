"use strict";

function lastCodes(value) {
    let first;
    let second;
    for (let index = 0; index < 3; index++) {
        first = value.charCodeAt(index);
        second = value.charCodeAt(index);
    }
    return first + second;
}

console.log(lastCodes("abc"));

const originalCharCodeAt = String.prototype.charCodeAt;
const originalStringPrototypeParent = Object.getPrototypeOf(String.prototype);

function mutateStringPrototype() {
    String.prototype.charCodeAt = function () {
        return 7;
    };
}

function sumWithUnknownMutation(value) {
    let first;
    let second;
    for (let index = 0; index < 3; index++) {
        first = value.charCodeAt(index);
        if (index === 0) {
            mutateStringPrototype();
        }
        second = value.charCodeAt(index);
    }
    return first + second;
}

console.log(sumWithUnknownMutation("abc"));

delete String.prototype.charCodeAt;
Object.setPrototypeOf(String.prototype, {
    charCodeAt(index) {
        return 20 + index;
    }
});
console.log(lastCodes("abc"));

Object.setPrototypeOf(String.prototype, originalStringPrototypeParent);
Object.defineProperty(String.prototype, "charCodeAt", {
    configurable: true,
    writable: true,
    value: originalCharCodeAt
});

const boxed = new String("abc");
Object.defineProperty(boxed, "charCodeAt", {
    configurable: true,
    get() {
        return function (index) {
            return 30 + index;
        };
    }
});
console.log(lastCodes(boxed));
