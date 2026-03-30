"use strict";

class Greeter {
  constructor(name) {
    this.name = name;
  }
  hello() {
    // call another instance method
    return this.prefix() + this.name;
  }
  prefix() {
    return "Hello, ";
  }
  logHello() {
    console.log(this.hello());
  }
}

const g = new Greeter("World");
g.logHello();
