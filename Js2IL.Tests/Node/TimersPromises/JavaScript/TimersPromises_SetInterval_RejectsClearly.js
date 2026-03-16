"use strict";

async function main(timersPromises) {
  try {
    await timersPromises.setInterval(0, "tick");
    console.log("setInterval-supported");
  } catch (error) {
    console.log(error.name);
    console.log(error.message);
  }
}

main(require("node:timers/promises"));
