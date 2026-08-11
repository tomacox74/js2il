"use strict";

class Example {
    #value;

    constructor(value) {
        this.#value = value;
    }

    static hasBrand(value) {
        return #value in value;
    }

    static read(value) {
        return value.#value;
    }

    static write(value, next) {
        value.#value = next;
    }
}

class Derived extends Example {}

console.log(Example.hasBrand(new Example()));
console.log(Example.hasBrand(new Derived()));
console.log(Example.hasBrand({}));
const example = new Example(1);
console.log(Example.read(example));
Example.write(example, 2);
console.log(Example.read(example));
