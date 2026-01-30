"use strict";

const global = 7;
console.log("before try");
try {
  const local = global + 4;
  console.log("Inside catch.  Calculated value is", local);
} catch {
  console.log("inside catch.. we should not be here");
}
console.log("try/catch finished.");
