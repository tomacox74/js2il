// Library module that exports an object instance with methods

class Calculator {
    add(a, b) {
        return a + b;
    }
    
    multiply(a, b) {
        return a * b;
    }
}

// Export an instance of the class
module.exports = new Calculator();
