"use strict";

const timersPromises = require("node:timers/promises");

async function main() {
  const iterator = timersPromises.setInterval(0, "tick");

  const first = await iterator.next();
  console.log(first.value);
  console.log(first.done);

  await timersPromises.setTimeout(0);
  await timersPromises.setTimeout(0);

  const second = await iterator.next();
  const third = await iterator.next();
  console.log(second.value);
  console.log(second.done);
  console.log(third.value);
  console.log(third.done);

  const closed = await iterator.return();
  console.log(closed.done);

  const after = await iterator.next();
  console.log(after.done);
}

main();
