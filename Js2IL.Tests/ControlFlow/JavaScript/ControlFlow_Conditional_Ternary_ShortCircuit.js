"use strict";\r\n\r\nlet x = 0;
function bump() {
  x = x + 1;
  return x;
}

// Only the chosen branch should execute
const a = (1) ? bump() : bump();
console.log(a);
console.log(x);
