"use strict";\r\n\r\n// Test PL5.5: Instance methods without explicit return should return 'undefined', not 'this'
// This is different from JavaScript convention where methods without return give undefined

class Calculator {
    constructor(value) {
        this.value = value;
    }
    
    // Method with no return statement - should return undefined
    doNothing() {
        const temp = this.value * 2;
    }
    
    // Method with explicit return undefined
    returnsUndefined() {
        return undefined;
    }
    
    // Method with explicit return
    returnsValue() {
        return this.value;
    }
    
    // Method with explicit return this
    returnsThis() {
        return this;
    }
}

const calc = new Calculator(10);

// Test method without return
const result1 = calc.doNothing();
console.log(result1 === undefined ? "undefined" : "not undefined");

// Test method returning explicit undefined
const result2 = calc.returnsUndefined();
console.log(result2 === undefined ? "undefined" : "not undefined");

// Test method returning value
const result3 = calc.returnsValue();
console.log(result3);

// Test method returning this
const result4 = calc.returnsThis();
console.log(result4 === calc ? "same instance" : "different");
