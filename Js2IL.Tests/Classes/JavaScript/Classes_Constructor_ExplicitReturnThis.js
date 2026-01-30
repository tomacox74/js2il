"use strict";\r\n\r\n// Test PL5.4a: Constructor with explicit return statements
// Both 'return;' and 'return this;' in constructors should work correctly
// Constructors are void-returning in IL, so return statements don't push values

class WithExplicitReturn {
    constructor() {
        this.value = 100;
        return this; // Explicit return of this - valid in JS constructors
    }
}

class WithVoidReturn {
    constructor() {
        this.value = 200;
        return; // void return - valid in JS constructors
    }
}

// This should work correctly
const w = new WithExplicitReturn();
console.log(w.value);
