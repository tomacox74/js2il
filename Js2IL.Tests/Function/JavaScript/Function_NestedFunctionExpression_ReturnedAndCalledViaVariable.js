"use strict";\r\n\r\nfunction makeAdder(n) {
  return function (x) {
    return n + x;
  };
}

let add5 = makeAdder(5);
console.log(add5(3));
