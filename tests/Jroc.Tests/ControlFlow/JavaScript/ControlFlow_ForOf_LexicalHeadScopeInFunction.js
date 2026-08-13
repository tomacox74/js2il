"use strict";

function probe() {
  let headProbe;

  for (let [x] of (headProbe = function () { return typeof x; }, [])) {
  }

  try {
    headProbe();
  } catch (error) {
    return error instanceof ReferenceError;
  }

  return false;
}

console.log(probe());
