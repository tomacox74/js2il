"use strict";

class Base {
  constructor() {
    this.x = 1;
  }
}

class Derived extends Base {
  constructor() {
    super();
    // Re-assign the same property name in derived.
    this.x = 2;
    this.y = 3;
  }
}

const o = new Derived();
const keys = [];
for (const k in o) {
  keys.push(k);
}
keys.sort();
console.log(keys.join(','));
