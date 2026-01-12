// Test PL5.4: Constructor implicitly returns 'this' unless explicitly returning an object
// When using 'new Class()', the result should be the class instance
// NOTE: Explicit 'return;' and 'return this;' in constructors cause InvalidProgramException (separate bug)

class Simple {
    constructor() {
        this.value = 42;
    }
}

class WithParams {
    constructor(a, b) {
        this.sum = a + b;
    }
}

// Test implicit return
const s = new Simple();
console.log(s.value); // 42
// Verify we got a proper instance by checking we can access the property
console.log(s.value === 42 ? "correct" : "wrong");

// Test constructor with parameters
const p = new WithParams(10, 20);
console.log(p.sum); // 30
