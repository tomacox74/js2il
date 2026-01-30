"use strict";\r\n\r\nlet g = 0;

function outer() {
  let x = 1;

  function mid() {
    function inner() {
      g = g + 1;
      x = x + 1;
    }

    inner();
    inner();

    console.log(g);
    console.log(x);
  }

  mid();
}

outer();
