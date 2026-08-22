"use strict";

Array.prototype.scanPhaseTwo = function (onVisit) {
    let result = "";
    for (let index = 0; index < this.length; index++) {
        result += this[index];
        if (onVisit) {
            onVisit(this, index);
        }
    }
    return result;
};

const values = ["a", "b"];
console.log(values.scanPhaseTwo(function (array, index) {
    if (index === 0) {
        array.push("c");
    }
}));

const descriptorArray = ["unused"];
Object.defineProperty(descriptorArray, "0", {
    configurable: true,
    get() {
        return "getter";
    }
});
console.log(descriptorArray.scanPhaseTwo());

const sparse = [];
sparse.length = 2;
Array.prototype[1] = "prototype";
console.log(sparse.scanPhaseTwo());
delete Array.prototype[1];

const scanPhaseTwo = Array.prototype.scanPhaseTwo;
console.log(scanPhaseTwo.call({
    0: "ordinary",
    length: 1
}));

const proxy = new Proxy({
    0: "proxy",
    length: 1
}, {
    get(target, key) {
        return target[key];
    }
});
console.log(scanPhaseTwo.call(proxy));

class DerivedArray extends Array {
}
const derived = new DerivedArray();
derived.push("subclass");
console.log(derived.scanPhaseTwo());
