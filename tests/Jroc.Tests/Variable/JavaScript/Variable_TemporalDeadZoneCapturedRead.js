"use strict";

function readValue() {
  console.log(value);
}

try {
  readValue();
  console.log("NO_TDZ");
} catch (e) {
  console.log("TDZ_CAPTURED");
}

let value = "ready";
console.log(value);
