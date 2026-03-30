"use strict";

// Test default parameter values in class methods
class Calculator {
    add(a, b = 10) {
        console.log(a + b);
    }
    
    multiply(x = 5, y = 3, z = 2) {
        console.log(x * y * z);
    }
    
    greet(name = "World") {
        console.log("Hello, " + name);
    }
    
    calculate(a, b = a * 2, c = a + b) {
        console.log(c);
    }
}

const calc = new Calculator();

// Test with no arguments (defaults)
calc.greet();

// Test with argument provided
calc.greet("Alice");

// Test with partial arguments
calc.add(5);
calc.add(5, 15);

// Test with multiple defaults
calc.multiply();
calc.multiply(2);
calc.multiply(2, 4);
calc.multiply(2, 4, 3);

// Test method with parameter referencing other parameters
calc.calculate(5);
calc.calculate(5, 8);
calc.calculate(5, 8, 20);
