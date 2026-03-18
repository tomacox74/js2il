"use strict";

async function main(timersPromises) {
  const value = await timersPromises.setImmediate("immediate-value");
  console.log(value);
}

main(require("node:timers/promises"));
