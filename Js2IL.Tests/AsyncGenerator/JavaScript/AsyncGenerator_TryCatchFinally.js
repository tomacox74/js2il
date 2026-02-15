"use strict";

async function* g() {
  try {
    console.log("try1");
    yield 1;
    console.log("try2");
    yield await Promise.resolve(2);
  } catch (e) {
    console.log("catch=" + e);
    yield 3;
  } finally {
    console.log("finally");
  }
  console.log("after");
  yield 4;
}

(async () => {
  const it = g();

  const r1 = await it.next();
  console.log("r1=" + r1.value);

  const r2 = await it.next();
  console.log("r2=" + r2.value);

  const r3 = await it.next();
  console.log("r3=" + r3.value);

  const r4 = await it.next();
  console.log("r4=" + r4.value + " done=" + r4.done);
})();
