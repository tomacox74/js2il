'use strict';

class Animal {
    constructor(name) {
        this.name = name;
    }
    speak() {
        return this.name + ' makes a sound.';
    }
}

class Dog extends Animal {
    constructor(name) {
        super(name);
    }
    speak() {
        return this.name + ' barks.';
    }
}

const dog = new Dog('Rex');
console.log(dog.speak());
console.log('CANARY:classes:ok');
