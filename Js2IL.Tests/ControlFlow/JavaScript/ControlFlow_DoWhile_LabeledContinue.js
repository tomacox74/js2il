"use strict";

let i = 0;

outer: do {
  i++;
  for (let j = 0; j < 1; j++) {
    if (i === 2) {
      continue outer;
    }
    console.log(i);
  }
} while (i < 3);
