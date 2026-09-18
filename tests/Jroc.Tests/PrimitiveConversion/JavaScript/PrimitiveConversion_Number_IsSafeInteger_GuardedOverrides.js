"use strict";

console.log(Number.isSafeInteger(1));

const numberAlias = Number;
numberAlias.isSafeInteger = () => "property";
console.log(Number.isSafeInteger(1));

Number = {
    isSafeInteger() {
        return "binding";
    }
};
console.log(Number.isSafeInteger(1));
