"use strict";

let value = "outer";

{
  try {
    console.log(value);
    console.log("NO_TDZ");
  } catch (e) {
    console.log("INNER_TDZ");
  }

  let value = "inner";
  console.log(value);
}

console.log(value);
