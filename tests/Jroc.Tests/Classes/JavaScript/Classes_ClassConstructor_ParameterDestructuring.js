"use strict";

// Test parameter destructuring in class constructors
class Point {
    constructor({ x, y }) {
        this.x = x;
        this.y = y;
    }
    
    display() {
        console.log("Point: (" + this.x + ", " + this.y + ")");
    }
}

class Person {
    constructor({ name, age, city }) {
        this.name = name;
        this.age = age;
        this.city = city;
    }
    
    greet() {
        console.log(this.name + " from " + this.city + " is " + this.age);
    }
}

class Config {
    constructor({ host = "localhost", port = 8080, secure = false }) {
        this.host = host;
        this.port = port;
        this.secure = secure;
    }
    
    display() {
        console.log((this.secure ? "https://" : "http://") + this.host + ":" + this.port);
    }
}

// Test basic destructuring
const p1 = new Point({ x: 10, y: 20 });
p1.display();

const p2 = new Point({ x: -5, y: 15 });
p2.display();

// Test multiple properties
const person1 = new Person({ name: "Alice", age: 30, city: "Seattle" });
person1.greet();

const person2 = new Person({ name: "Bob", age: 25, city: "Portland" });
person2.greet();

// Test with defaults in destructured parameters
const cfg1 = new Config({ host: "example.com" });
cfg1.display();

const cfg2 = new Config({ host: "api.test.com", port: 3000, secure: true });
cfg2.display();

const cfg3 = new Config({});
cfg3.display();
