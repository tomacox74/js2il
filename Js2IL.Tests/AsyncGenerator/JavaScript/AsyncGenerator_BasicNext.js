"use strict";

async function* g() {
  yield 1;
  yield await Promise.resolve(2);
  return 3;
}

(async () => {
  const it = g();

  const r1 = await it.next();
  console.log(`v1: ${r1.value} done: ${r1.done}`);

  const r2 = await it.next();
  console.log(`v2: ${r2.value} done: ${r2.done}`);

  const r3 = await it.next();
  console.log(`v3: ${r3.value} done: ${r3.done}`);
})();
