"use strict";

const timersPromises = require("node:timers/promises");

async function main() {
  const iterator = timersPromises.setInterval(Number.POSITIVE_INFINITY, "tick");

  const first = await iterator.next();
  console.log(first.value);
  console.log(first.done);

  const closed = await iterator.return();
  console.log(closed.done);
}

main();
