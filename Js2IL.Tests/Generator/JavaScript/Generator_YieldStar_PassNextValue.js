"use strict";\r\n\r\nfunction* inner() {
  const x = yield 1;
  yield x;
  return 99;
}

function* outer() {
  const r = yield* inner();
  console.log("r=" + r);
  return "done";
}

const it = outer();

let a = it.next();
console.log(a.value);
console.log(a.done);

let b = it.next(42);
console.log(b.value);
console.log(b.done);

let c = it.next();
console.log(c.value);
console.log(c.done);
