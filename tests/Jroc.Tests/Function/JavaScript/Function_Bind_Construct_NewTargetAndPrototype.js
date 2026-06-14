"use strict";

const boundThis = { marker: "bound" };

function C(a, b) {
    console.log(new.target === C);
    console.log(this === boundThis);
    this.sum = a + b;
}

C.prototype.kind = "ctor";

const Bound = C.bind(boundThis, 5);

console.log(Bound.length);
console.log(Bound.name);
console.log(Object.hasOwn(Bound, "prototype"));
console.log(Bound.prototype === undefined);

const instance = new Bound(7);
console.log(instance.sum);
console.log(instance.kind);
console.log(Object.getPrototypeOf(instance) === C.prototype);
