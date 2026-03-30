"use strict";

async function* g() {
  try {
    console.log("try1");
    yield 1;
    console.log("try2");
    yield 2;
  } finally {
    console.log("finally");
  }
}

(async () => {
  const it = g();

  const r1 = await it.next();
  console.log("r1.value=" + r1.value);
  console.log("r1.done=" + r1.done);

  const r2 = await it.return(99);
  console.log("r2.value=" + r2.value);
  console.log("r2.done=" + r2.done);

  const r3 = await it.next();
  console.log("r3.value=" + r3.value);
  console.log("r3.done=" + r3.done);
})();
