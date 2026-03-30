"use strict";

const util = require('util');

// Parent constructor
function Animal(name) {
    this.name = name;
}

Animal.prototype.speak = function() {
    return this.name + ' makes a sound';
};

// Child constructor
function Dog(name, breed) {
    this.name = name;
    this.breed = breed;
}

// Set up inheritance
util.inherits(Dog, Animal);

// Test that util.inherits completes without error
console.log('inherits completed');

// Test that super_ is set
console.log(Dog.super_ === Animal);

// Test basic dog instance
const dog = new Dog('Buddy', 'Golden Retriever');
console.log(dog.name);
console.log(dog.breed);
