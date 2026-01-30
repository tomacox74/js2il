"use strict";\r\n\r\nlet arr = ["a", "b", "c"]; 
for (const x of arr) {
  console.log(x);
  if (x === "b") {
    break;
  }
}
