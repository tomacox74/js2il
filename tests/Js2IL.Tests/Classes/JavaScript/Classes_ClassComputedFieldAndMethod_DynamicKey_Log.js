"use strict";

const fieldKey = "value";
const methodKey = "bump";

class Example {
  [fieldKey] = 41;

  [methodKey]() {
    return this[fieldKey] + 1;
  }
}

const example = new Example();
console.log(example[fieldKey]);
console.log(example[methodKey]());
console.log(example.value);
console.log(example.fieldKey === undefined);
