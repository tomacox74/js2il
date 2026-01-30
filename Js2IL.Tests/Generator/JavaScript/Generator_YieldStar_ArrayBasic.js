"use strict";\r\n\r\nfunction* g() {
  yield* [1, 2, 3];
  return 99;
}

const it = g();

let r1 = it.next();
console.log(r1.value);
console.log(r1.done);

let r2 = it.next();
console.log(r2.value);
console.log(r2.done);

let r3 = it.next();
console.log(r3.value);
console.log(r3.done);

let r4 = it.next();
console.log(r4.value);
console.log(r4.done);
