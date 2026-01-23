// Library module that exports a class constructor

class Calculator {
    add(a, b) {
        return a + b;
    }
    
    multiply(a, b) {
        return a * b;
    }
}

// Export the class (the importing module should `new` it)
module.exports = Calculator;
