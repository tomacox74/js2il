"use strict";

function makeCounter() {
    let value = 0;
    return {
        increment: function () {
            value++;
            return value;
        },
        read: () => value
    };
}

const firstCounter = makeCounter();
const secondCounter = makeCounter();
console.log("identity", firstCounter.increment === firstCounter.increment, firstCounter.increment !== secondCounter.increment);
console.log("capture", firstCounter.increment(), firstCounter.read(), secondCounter.read());

function readValue() {
    return this.value;
}

const lexicalReceiver = {
    value: 9,
    createReader: function () {
        return () => this.value;
    }
};

console.log("this", readValue.call({ value: 7 }), lexicalReceiver.createReader().call({ value: 100 }));

function sumRecursively(value, callback) {
    if (value === 0) {
        return 0;
    }

    return callback(value) + sumRecursively(value - 1, callback);
}

console.log("recursion", sumRecursively(4, value => value));

function Receiver(value) {
    this.value = value;
    this.wasConstructed = new.target === Receiver;
}

const receiver = new Receiver(13);
const arrow = () => 1;
let arrowConstructionThrows = false;
try {
    new arrow();
} catch (error) {
    arrowConstructionThrows = error instanceof TypeError;
}

console.log("construct", receiver.value, receiver.wasConstructed, arrowConstructionThrows, typeof arrow.prototype);

function AlternateReceiver() {}
AlternateReceiver.prototype.kind = "alternate";
const alternateReceiver = Reflect.construct(Receiver, [17], AlternateReceiver);
console.log(
    "newTarget",
    alternateReceiver.value,
    alternateReceiver.wasConstructed,
    Object.getPrototypeOf(alternateReceiver) === AlternateReceiver.prototype,
    alternateReceiver.kind);

const lengthDescriptor = Object.getOwnPropertyDescriptor(Receiver, "length");
console.log(
    "descriptor",
    lengthDescriptor.value,
    lengthDescriptor.writable,
    lengthDescriptor.enumerable,
    lengthDescriptor.configurable);

const proxied = new Proxy(
    function (value) {
        return value + 1;
    },
    {
        apply: function (target, thisArgument, argumentsList) {
            return Reflect.apply(target, thisArgument, argumentsList) + 1;
        }
    });

console.log("proxy", proxied(5));
console.log("callback", [1, 2, 3].map(value => value * 2).join(","));
console.log("replace", "abc".replace("b", function (match, offset) {
    return match.toUpperCase() + offset;
}));
