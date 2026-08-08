"use strict";

function add(a, b, c) {
    return this.base + a + b + c;
}

const bound = add.bind({ base: 10 }, 1);
const chained = bound.bind({ base: 100 }, 2);
console.log("calls", bound(3, 4), chained(3));
console.log("applyCall", bound.apply({ base: 200 }, [5, 6]), bound.call({ base: 300 }, 7, 8));
console.log("metadata", bound.name, bound.length, chained.name, chained.length);
console.log(
    "surface",
    typeof bound,
    Object.hasOwn(bound, "name"),
    Object.hasOwn(bound, "length"),
    Object.hasOwn(bound, "prototype"),
    bound.prototype === undefined);
console.log("toString", bound.toString().includes("bound add"));

const object = {
    value: 5,
    method(amount) {
        return this.value + amount;
    }
};
const boundMethod = object.method.bind({ value: 20 }, 2);
console.log("method", boundMethod(), boundMethod.call({ value: 100 }));

const arrow = (value) => value + 1;
const boundArrow = arrow.bind(null, 4);
let arrowConstructThrows = false;
try {
    Reflect.construct(boundArrow, []);
} catch (error) {
    arrowConstructThrows = error instanceof TypeError;
}
console.log("arrow", boundArrow(), arrowConstructThrows);

class Box {
    constructor(value, extra) {
        this.total = value + extra;
        this.seenNewTarget = new.target;
    }
}

const BoundBox = Box.bind({ ignored: true }, 7);
const box = new BoundBox(8);
console.log(
    "class",
    box.total,
    box.seenNewTarget === Box,
    box instanceof Box,
    box instanceof BoundBox,
    BoundBox.name,
    BoundBox.length,
    Object.hasOwn(BoundBox, "prototype"));

bound.call = function() {
    return "own call";
};
console.log("override", bound.call());
