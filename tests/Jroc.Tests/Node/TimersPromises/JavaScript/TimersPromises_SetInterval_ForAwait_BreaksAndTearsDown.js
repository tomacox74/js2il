"use strict";

const timersPromises = require("node:timers/promises");

async function main() {
  let count = 0;

  for await (const value of timersPromises.setInterval(0, "tick")) {
    console.log(value);
    count++;

    if (count === 3) {
      break;
    }
  }

  console.log("break-done");
}

main();
