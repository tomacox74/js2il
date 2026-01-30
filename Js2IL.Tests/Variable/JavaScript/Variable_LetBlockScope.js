"use strict";

let x = 1;
{
  let x = 2;
  console.log(x);
}
console.log(x);
