"use strict";

function makeObject(offset) {
    return {
        value: 3,

        add(amount) {
            return this.value + amount + offset;
        },

        get current() {
            return this.value + offset;
        },

        set current(value) {
            this.value = value - offset;
        },

        ["computed"](amount) {
            return this.value + amount;
        }
    };
}

const object = makeObject(2);
const addDescriptor = Object.getOwnPropertyDescriptor(object, "add");
const currentDescriptor = Object.getOwnPropertyDescriptor(object, "current");
const computedDescriptor = Object.getOwnPropertyDescriptor(object, "computed");

console.log("calls", object.add(4), addDescriptor.value.call({ value: 10 }, 5));
object.current = 12;
console.log("accessors", object.value, object.current, currentDescriptor.get.call(object));
currentDescriptor.set.call(object, 20);
console.log("setter", object.value);
console.log("computed", object.computed(1), computedDescriptor.value.call({ value: 30 }, 2));
console.log(
    "metadata",
    addDescriptor.value.name,
    addDescriptor.value.length,
    currentDescriptor.get.name,
    currentDescriptor.set.name,
    computedDescriptor.value.name);
console.log(
    "descriptors",
    addDescriptor.value === object.add,
    currentDescriptor.get === Object.getOwnPropertyDescriptor(object, "current").get,
    Object.hasOwn(addDescriptor.value, "prototype"));

let constructThrows = false;
try {
    Reflect.construct(addDescriptor.value, []);
} catch (error) {
    constructThrows = error instanceof TypeError;
}
console.log("nonconstructor", constructThrows);

const base = {
    label() {
        return "base";
    }
};
const derived = {
    __proto__: base,
    label() {
        return super.label() + ":derived";
    }
};
console.log("super", derived.label());

const first = makeObject(0);
const second = makeObject(0);
console.log("identity", first.add !== second.add);
