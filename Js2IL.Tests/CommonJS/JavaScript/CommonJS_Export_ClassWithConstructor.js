"use strict";\r\n\r\n// Test: Import a class constructor from another module and instantiate it
// This tests cross-module class constructor exports + `new` in the importing module

const Person = require('./CommonJS_Export_ClassWithConstructor_Lib');

// Create a default instance
const defaultPerson = new Person("Default", 25);
console.log("default greeting:", defaultPerson.greet());

// Create a new person
const alice = new Person("Alice", 30);
console.log("alice greeting:", alice.greet());
console.log("alice name:", alice.name);
console.log("alice age:", alice.age);
