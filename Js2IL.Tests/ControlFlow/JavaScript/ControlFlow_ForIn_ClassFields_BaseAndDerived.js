"use strict";\r\n\r\nclass Base {
  constructor() {
    this.base = 1;
  }
}

class Derived extends Base {
  constructor() {
    super();
    this.derived = 2;
  }
}

const o = new Derived();
const keys = [];
for (const k in o) {
  keys.push(k);
}
keys.sort();
console.log(keys.join(','));
