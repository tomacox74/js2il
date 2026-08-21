"use strict";

let useArray = true;
let receiver = useArray ? [] : {
    push(value) {
        return "object:" + value;
    }
};
console.log(receiver.push(1));

receiver.push = function (value) {
    return "own:" + value;
};
console.log(receiver.push(2));

delete receiver.push;
Object.defineProperty(receiver, "push", {
    configurable: true,
    get() {
        return function (value) {
            return "getter:" + value;
        };
    }
});
console.log(receiver.push(3));

delete receiver.push;
Object.setPrototypeOf(receiver, {
    push(value) {
        return "custom:" + value;
    }
});
console.log(receiver.push(4));

Object.setPrototypeOf(receiver, Array.prototype);
Array.prototype.push = function (value) {
    return "prototype:" + value;
};
console.log(receiver.push(5));
