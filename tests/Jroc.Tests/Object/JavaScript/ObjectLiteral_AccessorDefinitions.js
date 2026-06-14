"use strict";

const obj = {
  base: 10,
  get value() {
    return this.base + 1;
  },
  set value(v) {
    this.base = v * 2;
  },
  get ["double"]() {
    return this.base * 2;
  }
};

console.log(obj.value);
obj.value = 7;
console.log(obj.base);
console.log(obj.value);
console.log(obj.double);

const valueDesc = Object.getOwnPropertyDescriptor(obj, "value");
console.log(typeof valueDesc.get);
console.log(typeof valueDesc.set);
console.log(valueDesc.enumerable);
console.log(valueDesc.configurable);
console.log(Object.keys(obj).join(","));

const overwritten = {
  get x() {
    return 1;
  },
  x: 5
};
console.log(overwritten.x);

const spreadOverwrite = {
  get x() {
    return 1;
  },
  ...{ x: 9 }
};
console.log(spreadOverwrite.x);
