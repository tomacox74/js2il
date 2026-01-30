"use strict";\r\n\r\n// Test default parameter values in class constructors
class Person {
    constructor(name = "Unknown", age = 0) {
        this.name = name;
        this.age = age;
    }
    
    display() {
        console.log(this.name + " is " + this.age + " years old");
    }
}

class Box {
    constructor(width, height = 20, depth = 5) {
        this.width = width;
        this.height = height;
        this.depth = depth;
    }
    
    volume() {
        console.log(this.width * this.height * this.depth);
    }
}

// Test with no arguments (all defaults)
const p1 = new Person();
p1.display();

// Test with partial arguments
const p2 = new Person("Alice");
p2.display();

// Test with all arguments
const p3 = new Person("Bob", 25);
p3.display();

// Test Box - required param with defaults
const b1 = new Box(10);
b1.volume();

const b2 = new Box(5, 10);
b2.volume();

const b3 = new Box(5, 10, 3);
b3.volume();

// Test constructor with parameter references
class Rectangle {
    constructor(width, height = width) {
        this.width = width;
        this.height = height;
    }
    
    area() {
        console.log(this.width * this.height);
    }
}

const rect1 = new Rectangle(5);
rect1.area();

const rect2 = new Rectangle(5, 10);
rect2.area();
