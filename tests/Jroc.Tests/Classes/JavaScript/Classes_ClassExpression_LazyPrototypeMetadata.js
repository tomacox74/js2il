const Greeter = class {
  greet() {
    return "hello";
  }
};

const greeter = new Greeter();
console.log(greeter.greet());
console.log(Object.getPrototypeOf(greeter) === Greeter.prototype);
console.log(typeof Greeter.prototype.greet);
console.log(Object.keys(Greeter.prototype).length);
console.log(Object.getOwnPropertyDescriptor(Greeter.prototype, "greet").enumerable);
