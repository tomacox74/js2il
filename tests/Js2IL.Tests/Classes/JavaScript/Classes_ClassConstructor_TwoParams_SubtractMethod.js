"use strict";

class Subber {
    constructor(a, b) {
        this.a = a;
        this.b = b;
    }

    sub() {
        return this.a - this.b;
    }
}

const s = new Subber("Hello", "World");
const n = new Subber(10, 3);

console.log(s.sub());
console.log(n.sub());
