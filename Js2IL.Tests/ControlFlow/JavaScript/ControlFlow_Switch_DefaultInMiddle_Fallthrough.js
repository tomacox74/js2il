"use strict";

// Default is not "last"; it executes based on source order.
switch (1) {
  case 1:
    console.log("a");
  default:
    console.log("d");
  case 2:
    console.log("b");
    break;
}
