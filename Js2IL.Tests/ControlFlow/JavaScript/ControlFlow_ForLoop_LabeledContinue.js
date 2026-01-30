"use strict";\r\n\r\nouter: for (let i = 0; i < 3; i++) {
  for (let j = 0; j < 1; j++) {
    if (i === 1) {
      continue outer;
    }
    console.log(i);
  }
}
