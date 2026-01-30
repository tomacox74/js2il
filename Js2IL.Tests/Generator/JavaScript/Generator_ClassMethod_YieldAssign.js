"use strict";\r\n\r\nclass Gen {
  *values() {
    const x = yield 10;
    yield x;
    return 99;
  }
}

const g = new Gen();
const it = g.values();

let r;
r = it.next();
console.log("y1:", r.value, "done:", r.done);

r = it.next(42);
console.log("y2:", r.value, "done:", r.done);

r = it.next();
console.log("y3:", r.value, "done:", r.done);
