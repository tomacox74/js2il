"use strict";

function Receiver(value) {
    this.value = value;
    this.seenNewTarget = new.target;
}

const prototypeDescriptor = Object.getOwnPropertyDescriptor(Receiver, "prototype");
const constructorDescriptor = Object.getOwnPropertyDescriptor(Receiver.prototype, "constructor");
console.log(
    "prototypeDescriptor",
    prototypeDescriptor.writable,
    prototypeDescriptor.enumerable,
    prototypeDescriptor.configurable);
console.log(
    "constructorDescriptor",
    constructorDescriptor.value === Receiver,
    constructorDescriptor.writable,
    constructorDescriptor.enumerable,
    constructorDescriptor.configurable);

const direct = new Receiver(3);
console.log(
    "direct",
    direct.value,
    direct.seenNewTarget === Receiver,
    Object.getPrototypeOf(direct) === Receiver.prototype,
    direct instanceof Receiver);

const replacementPrototype = { kind: "replacement" };
Receiver.prototype = replacementPrototype;
const replaced = new Receiver(5);
console.log(
    "replacement",
    replaced.value,
    replaced.kind,
    Object.getPrototypeOf(replaced) === replacementPrototype,
    replaced instanceof Receiver);

Receiver.prototype = 7;
const fallback = new Receiver(6);
console.log(
    "fallback",
    fallback.value,
    Object.getPrototypeOf(fallback) === Object.prototype);

function Alternate() {}
Alternate.prototype = { alternate: true };
const alternate = Reflect.construct(Receiver, [9], Alternate);
console.log(
    "alternate",
    alternate.value,
    alternate.seenNewTarget === Alternate,
    alternate.alternate,
    Object.getPrototypeOf(alternate) === Alternate.prototype);

function ReturnsPrimitive() {
    this.kind = "receiver";
    return 42;
}

function ReturnsObject() {
    this.kind = "receiver";
    return { kind: "override" };
}

console.log("returns", new ReturnsPrimitive().kind, new ReturnsObject().kind);

class DerivedFromObject extends function () {
    return { replacement: true, baseNewTarget: new.target };
} {
}

const derivedObject = new DerivedFromObject();
console.log(
    "derivedObject",
    derivedObject.replacement,
    derivedObject.baseNewTarget === DerivedFromObject,
    derivedObject instanceof DerivedFromObject);

function BoundTarget(value) {
    this.value = value;
    this.seenNewTarget = new.target;
}

const Bound = BoundTarget.bind(null, 11);
const bound = new Bound();
console.log(
    "bound",
    bound.value,
    bound.seenNewTarget === BoundTarget,
    bound instanceof BoundTarget,
    bound instanceof Bound);

const arrow = () => {};
const holder = { method() {} };
let arrowThrows = false;
let methodThrows = false;
try {
    Reflect.construct(arrow, []);
} catch (error) {
    arrowThrows = error instanceof TypeError;
}
try {
    Reflect.construct(holder.method, []);
} catch (error) {
    methodThrows = error instanceof TypeError;
}
console.log("nonconstructors", arrowThrows, methodThrows);
