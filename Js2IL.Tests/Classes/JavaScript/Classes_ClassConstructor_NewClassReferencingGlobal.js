"use strict";

// Test: Class constructor instantiates another class that references a global variable
// This reproduces the bug where the constructor scope array construction fails
// when a nested class needs to access parent scopes.

const GLOBAL_VALUE = 42;

class Inner {
    constructor() {
        this.value = GLOBAL_VALUE;
    }
}

class Outer {
    constructor() {
        this.inner = new Inner();
    }
}

const obj = new Outer();
console.log(obj.inner.value);
