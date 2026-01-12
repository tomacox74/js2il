// Test PL5.4a: Constructor with explicit return statements
// BUG: Both 'return;' and 'return this;' generate InvalidProgramException
// because constructors in IL are void-returning

class WithExplicitReturn {
    constructor() {
        this.value = 100;
        return this; // Explicit return of this - generates invalid IL
    }
}

class WithVoidReturn {
    constructor() {
        this.value = 200;
        return; // void return - also generates invalid IL
    }
}

// This will fail with InvalidProgramException
const w = new WithExplicitReturn();
console.log(w.value);
