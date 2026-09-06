"use strict";

class Base {
  join(first, second, third) {
    return `${this.prefix}:${first},${second},${third}`;
  }
}

class Derived extends Base {
  constructor() {
    super();
    this.prefix = "derived";
  }

  join(...args) {
    return super.join(...args);
  }
}

const derived = new Derived();
console.log(derived.join(1, 2, 3));
