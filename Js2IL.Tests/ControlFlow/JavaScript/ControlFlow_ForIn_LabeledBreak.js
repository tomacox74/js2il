"use strict";

let obj = { a: 1, b: 2, c: 3 };

outer: for (let k in obj) {
  for (let j = 0; j < 1; j++) {
    console.log(k);
    if (k === "b") {
      break outer;
    }
  }
}
