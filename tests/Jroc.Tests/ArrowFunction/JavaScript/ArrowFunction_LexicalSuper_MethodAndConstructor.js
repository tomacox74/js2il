"use strict";

class Base {
    constructor(value) {
        this.value = value;
    }

    read() {
        return this.value;
    }
}

class Derived extends Base {
    constructor(value) {
        super(value);
        this.fromConstructor = () => super.read();
    }

    makeReader() {
        return () => super.read();
    }
}

const derived = new Derived(11);
const detachedConstructorArrow = derived.fromConstructor;
const detachedMethodArrow = derived.makeReader();

console.log(detachedConstructorArrow.call({ value: 99 }));
console.log(detachedMethodArrow.call({ value: 99 }));
