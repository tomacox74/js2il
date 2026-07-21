import { bench, run } from "../runner.mjs";

const values = Array.from({ length: 256 }, (_, index) => index + 1);

bench("numeric loop", () => {
  let total = 0;
  for (let index = 0; index < values.length; index++) {
    total += values[index] * 3;
  }
  return total;
});

bench("object property reads", () => {
  const value = { left: 17, right: 29 };
  let total = 0;
  for (let index = 0; index < 256; index++) {
    total += value.left + value.right;
  }
  return total;
});

await run();
