"use strict";

async function* g() {
  yield 1;
  yield 2;
}

(async () => {
  let i = 0;
  for await (const x of g()) {
    console.log(`x${++i}: ${x}`);
  }
})();
