// Bug repro: class method local variable initialized as number,
// then reassigned from another method call.
// The type inference marks 'factor' as stable double, but method calls return object.
// The IL generator must unbox before storing to the double local.

class Counter {
    constructor() {
        this.value = 0;
    }

    getNext() {
        // Use simple addition instead of increment to avoid separate bug
        this.value = this.value + 1;
        return this.value;
    }
}

class Calculator {
    constructor() {
        this.counter = new Counter();
    }

    compute() {
        let factor = 1;  // Inferred as stable double
        
        // This assignment should unbox the method result before storing to 'factor'
        factor = this.counter.getNext();
        
        console.log("factor after getNext:", factor);
        
        // Use factor in a calculation to verify it works
        let result = factor * 2;
        console.log("result:", result);
        
        return result;
    }
}

const calc = new Calculator();
const output = calc.compute();
console.log("final output:", output);
