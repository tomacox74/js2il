"use strict";

async function* g() {
  yield 1;
  const x = await Promise.resolve(2);
  yield x + 1;
  yield await Promise.resolve(4);
}

(async () => {
  const it = g();

  const r1 = await it.next();
  console.log("r1.value=" + r1.value);
  console.log("r1.done=" + r1.done);

  const r2 = await it.next();
  console.log("r2.value=" + r2.value);
  console.log("r2.done=" + r2.done);

  const r3 = await it.next();
  console.log("r3.value=" + r3.value);
  console.log("r3.done=" + r3.done);

  const r4 = await it.next();
  console.log("r4.value=" + r4.value);
  console.log("r4.done=" + r4.done);
})();
