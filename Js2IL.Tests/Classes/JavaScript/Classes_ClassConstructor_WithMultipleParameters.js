"use strict";

class C1 {
    constructor(a) {
        console.log("C1", a);
    }
}

class C2 {
    constructor(a, b) {
        console.log("C2", a, b);
    }
}

class C3 {
    constructor(a, b, c) {
        console.log("C3", a, b, c);
    }
}

class C4 {
    constructor(a, b, c, d) {
        console.log("C4", a, b, c, d);
    }
}

class C5 {
    constructor(a, b, c, d, e) {
        console.log("C5", a, b, c, d, e);
    }
}

class C6 {
    constructor(a, b, c, d, e, f) {
        console.log("C6", a, b, c, d, e, f);
    }
}

const c1 = new C1(1);
const c2 = new C2(1, 2);
const c3 = new C3(1, 2, 3);
const c4 = new C4(1, 2, 3, 4);
const c5 = new C5(1, 2, 3, 4, 5);
const c6 = new C6(1, 2, 3, 4, 5, 6);
