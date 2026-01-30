"use strict";

class Counter {
  constructor() {
    this.count = 7;
  }

  *getCountLater() {
    yield 0;
    return this.count;
  }
}

const c = new Counter();
const it = c.getCountLater();

let r;
r = it.next();
console.log("t1:", r.value, "done:", r.done);

r = it.next();
console.log("t2:", r.value, "done:", r.done);
