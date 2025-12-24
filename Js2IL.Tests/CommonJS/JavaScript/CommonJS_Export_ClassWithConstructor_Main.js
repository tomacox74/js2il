// Test: Import factory function and default instance from another module
// This tests cross-module exports with factory pattern

const PersonModule = require('./CommonJS_Export_ClassWithConstructor_Lib');

// Use the default exported instance
const defaultPerson = PersonModule.defaultPerson;
console.log("default greeting:", defaultPerson.greet());

// Use the factory function to create a new person
const alice = PersonModule.createPerson("Alice", 30);
console.log("alice greeting:", alice.greet());
console.log("alice name:", alice.name);
console.log("alice age:", alice.age);
