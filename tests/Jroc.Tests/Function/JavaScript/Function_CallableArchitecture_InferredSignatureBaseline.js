"use strict";

function returnNumber() {
    return 42;
}

function returnBoolean() {
    return true;
}

function negate(value) {
    return !value;
}

function returnString() {
    return "typed!";
}

function stringLength(value) {
    return value.length;
}

function identity(value) {
    return value;
}

console.log(returnNumber());
console.log(returnBoolean());
console.log(negate(true));
console.log(returnString());
console.log(stringLength("typed"));
console.log(identity(1));
console.log(identity("dynamic"));
