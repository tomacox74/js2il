"use strict";

function Point(x, y) {
    this.x = x;
    this.y = y;
}

Point.prototype.sum = function () {
    return this.x + this.y;
};

const point = new Point(2, 3);
point.x = 5;
point.z = 4;
console.log(point.sum());
console.log(point instanceof Point);

const descriptor = Object.getOwnPropertyDescriptor(point, "x");
console.log(descriptor.value, descriptor.writable, descriptor.enumerable, descriptor.configurable);
console.log(Object.keys(point).join(","));
console.log(delete point.y);
console.log(Object.keys(point).join(","));
console.log(point.y === undefined);
point.y = 8;
console.log(Object.keys(point).join(","));
console.log(Object.getPrototypeOf(point) === Point.prototype);

const bridge = Object.create(Point.prototype);
bridge.kind = "bridge";
Object.setPrototypeOf(point, bridge);
console.log(point.kind);
console.log(point instanceof Point);

function ReturnObject() {
    this.discarded = true;
    return { override: "object" };
}

function ReturnPrimitive() {
    this.kept = "receiver";
    return 17;
}

console.log(new ReturnObject().override);
console.log(new ReturnPrimitive().kept);

function makeConstructor(offset) {
    return function (value) {
        this.value = value + offset;
    };
}

const Closed = makeConstructor(10);
console.log(new Closed(5).value);

const BoundPoint = Point.bind(null, 10);
const boundPoint = new BoundPoint(5);
console.log(boundPoint.sum());
console.log(boundPoint instanceof Point);

const Dynamic = new Function("value", "this.value = value;");
const dynamicInstance = new Dynamic(9);
console.log(dynamicInstance.value);
console.log(dynamicInstance instanceof Dynamic);

function PrimitivePrototype() {}
PrimitivePrototype.prototype = 3;
console.log(Object.getPrototypeOf(new PrimitivePrototype()) === Object.prototype);

function NullPrototype() {}
NullPrototype.prototype = null;
console.log(Object.getPrototypeOf(new NullPrototype()) === Object.prototype);

class PointReader {
    static read(value) {
        return value.sum();
    }
}

console.log(PointReader.read(new Point(6, 1)));
