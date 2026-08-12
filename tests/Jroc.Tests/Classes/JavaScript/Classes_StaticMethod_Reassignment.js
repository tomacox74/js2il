"use strict";

class Example {
  static identify() {
    return "original";
  }
}

Example.identify = () => "replacement";
console.log(Example.identify());
