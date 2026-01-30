"use strict";\r\n\r\nlet arr = ["a", "b", "c"]; 

outer: for (const x of arr) {
  for (let j = 0; j < 1; j++) {
    console.log(x);
    if (x === "b") {
      break outer;
    }
  }
}
