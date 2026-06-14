"use strict";

switch (1) {
  case 1:
    console.log("outer");

    switch (2) {
      case 2:
        console.log("inner");
        break;
      default:
        console.log("innerdefault");
    }

    console.log("afterinner");
    break;

  default:
    console.log("outerdefault");
}

console.log("done");
