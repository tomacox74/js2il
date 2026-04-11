"use strict";

class Example {
  ["field"] = 7;
  ["method"]() {
    return this.field + 2;
  }
}

const example = new Example();
console.log(example.field);
console.log(example.method());
