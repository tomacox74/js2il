"use strict";

const lengthCoercion = [1, 2];
const freezingLength = {
    valueOf: function () {
        Object.freeze(lengthCoercion);
        return 0;
    }
};
try {
    lengthCoercion.length = freezingLength;
} catch (error) {
    console.log(error.name);
}
console.log(lengthCoercion.length, lengthCoercion.join(","));

const spliceCoercion = [1, 2, 3];
const clearingStart = {
    valueOf: function () {
        spliceCoercion.length = 0;
        return 0;
    }
};
const coercionRemoved = spliceCoercion.splice(clearingStart, 1);
console.log(spliceCoercion.length, coercionRemoved.length);

const warmed = [];
warmed.push("warm");
let inheritedPushValue = 0;
Object.defineProperty(Array.prototype, "0", {
    set: function (value) { inheritedPushValue = value; },
    configurable: true
});
const inheritedPush = [];
inheritedPush.push(42);
console.log(inheritedPush.length, Object.hasOwn(inheritedPush, "0"), inheritedPushValue);
delete Array.prototype[0];

let customPushValue = 0;
const customPrototype = {};
Object.defineProperty(customPrototype, "0", {
    set: function (value) { customPushValue = value; },
    configurable: true
});
const customPrototypeArray = [];
Object.setPrototypeOf(customPrototypeArray, customPrototype);
customPrototypeArray.push(84);
console.log(customPrototypeArray.length, Object.hasOwn(customPrototypeArray, "0"), customPushValue);

const sparseWritable = [];
sparseWritable[2000] = "before";
Object.preventExtensions(sparseWritable);
sparseWritable[2000] = "after";
console.log(sparseWritable.length, sparseWritable[2000]);
