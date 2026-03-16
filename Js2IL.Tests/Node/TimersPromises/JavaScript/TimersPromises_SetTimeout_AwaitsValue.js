"use strict";

async function main(timersPromises) {
  const value = await timersPromises.setTimeout(0, "timeout-value");
  console.log(value);
}

main(require("node:timers/promises"));
