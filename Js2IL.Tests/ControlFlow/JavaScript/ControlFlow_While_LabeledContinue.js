"use strict";\r\n\r\nlet i = 0;

outer: while (i < 3) {
  i++;
  for (let j = 0; j < 1; j++) {
    if (i === 2) {
      continue outer;
    }
    console.log(i);
  }
}
