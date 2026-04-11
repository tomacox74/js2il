"use strict";

outer: {
  switch (2) {
    case 1:
      console.log("x");
      break;
    case 2:
      console.log("a");
      break outer;
  }

  // Should not execute
  console.log("b");
}

console.log("c");
