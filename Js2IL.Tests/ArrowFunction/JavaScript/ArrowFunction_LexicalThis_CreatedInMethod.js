"use strict";

class Counter {
    constructor(initial) {
        this.x = initial;
    }

    makeGetter() {
        return () => this.x;
    }
}

const c = new Counter(7);
const g = c.makeGetter();

const other = { x: 99, g };
const another = { x: 1, g };

console.log(other.g());
console.log(another.g());
