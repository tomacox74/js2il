"use strict";\r\n\r\nfunction* inner() {
  yield 1;
  yield 2;
  return 3;
}

function* outer() {
  const r = yield* inner();
  console.log("r=" + r);
  return 4;
}

const it = outer();

let a = it.next();
console.log(a.value);
console.log(a.done);

let b = it.next();
console.log(b.value);
console.log(b.done);

let c = it.next();
console.log(c.value);
console.log(c.done);

let d = it.next();
console.log(d.value);
console.log(d.done);
