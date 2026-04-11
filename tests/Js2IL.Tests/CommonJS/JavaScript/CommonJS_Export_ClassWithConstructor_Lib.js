"use strict";

// Library module that exports a class constructor

class Person {
    constructor(name, age) {
        this.name = name;
        this.age = age;
    }
    
    greet() {
        return "Hello, I am " + this.name + " and I am " + this.age + " years old.";
    }
}

// Export the class (the importing module should `new` it)
module.exports = Person;
