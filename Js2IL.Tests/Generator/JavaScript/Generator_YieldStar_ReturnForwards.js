"use strict";\r\n\r\nfunction* inner() {
  yield 1;
  yield 5;
  return 7;
}

function* outer() {
  const r = yield* inner();
  console.log("r=" + r);
  yield 2;
}

const it = outer();

let a = it.next();
console.log(a.value);
console.log(a.done);

let b = it.return(123);
console.log(b.value);
console.log(b.done);
