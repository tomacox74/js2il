"use strict";

class Example {
  static value = 1;
  static {
    Example.value = Example.value + 2;
    console.log("block:" + Example.value);
  }
  static after = Example.value + 3;
}

console.log(Example.value);
console.log(Example.after);
