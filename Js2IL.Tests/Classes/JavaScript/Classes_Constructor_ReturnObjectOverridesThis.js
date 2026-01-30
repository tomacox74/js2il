"use strict";

class A {
    constructor() {
        this.a = 1;
        return { b: 2 };
    }
}

class B {
    constructor() {
        this.a = 1;
        return 1;
    }
}

let a = new A();
console.log(a.a);
console.log(a.b);

let b = new B();
console.log(b.a);
