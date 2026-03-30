"use strict";

// Test parameter destructuring in class methods
class Calculator {
    add({ a, b }) {
        return a + b;
    }
    
    multiply({ x, y }) {
        return x * y;
    }
}

class Formatter {
    formatPerson({ name, age, city }) {
        return name + " from " + city + " (age: " + age + ")";
    }
    
    formatDate({ year, month, day }) {
        return year + "-" + month + "-" + day;
    }
}

class Config {
    setConnection({ host = "localhost", port = 8080, secure = false }) {
        this.url = (secure ? "https://" : "http://") + host + ":" + port;
        return this.url;
    }
}

// Test basic destructuring in methods
const calc = new Calculator();
console.log(calc.add({ a: 5, b: 3 }));
console.log(calc.multiply({ x: 4, y: 7 }));

// Test multiple properties
const fmt = new Formatter();
console.log(fmt.formatPerson({ name: "Alice", age: 30, city: "Seattle" }));
console.log(fmt.formatDate({ year: 2025, month: 11, day: 30 }));

// Test with defaults in destructured parameters
const cfg = new Config();
console.log(cfg.setConnection({ host: "example.com" }));
console.log(cfg.setConnection({ host: "api.test.com", port: 3000, secure: true }));
console.log(cfg.setConnection({}));
