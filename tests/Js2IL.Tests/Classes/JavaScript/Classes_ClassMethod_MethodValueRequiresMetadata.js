class Greeter {
  greet() {
    return "hello";
  }

  getGreet() {
    return this.greet;
  }
}

const greeter = new Greeter();
console.log(typeof greeter.getGreet());
