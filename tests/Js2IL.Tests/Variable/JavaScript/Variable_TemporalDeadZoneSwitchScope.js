"use strict";

switch (0) {
  default:
    try {
      console.log(value);
      console.log("NO_TDZ");
    } catch (e) {
      console.log("SWITCH_TDZ");
    }

    let value = "ready";
    console.log(value);
    break;
}
