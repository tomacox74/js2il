"use strict";\r\n\r\nclass Counter {
    constructor(initial) {
        this.x = initial;
        this.getX = () => this.x;
    }
}

const c = new Counter(5);
const other = { x: 9, getX: c.getX };

console.log(c.getX());
console.log(other.getX());
