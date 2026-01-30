"use strict";\r\n\r\nclass Adder {
    constructor(a, b) {
        this.a = a;
        this.b = b;
    }

    add() {
        return this.a + this.b;
    }
}

const s = new Adder("Hello", "World");
const n = new Adder(5, 7);

console.log(s.add());
console.log(n.add());
