"use strict";

let receiver = {
    value() {
        return "original";
    }
};

console.log(receiver.value());
receiver.value = function () {
    return "override";
};
console.log(receiver.value());
