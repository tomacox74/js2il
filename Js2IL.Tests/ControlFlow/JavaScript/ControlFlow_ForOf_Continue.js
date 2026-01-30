"use strict";\r\n\r\nlet arr = ["a", "b", "c"]; 
for (const x of arr) {
  if (x === "b") {
    continue;
  }
  console.log(x);
}
