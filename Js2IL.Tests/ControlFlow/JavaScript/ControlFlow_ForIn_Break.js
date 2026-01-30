"use strict";\r\n\r\nlet obj = { a: 1, b: 2, c: 3 };
for (let k in obj) {
  console.log(k);
  if (k === "b") {
    break;
  }
}
