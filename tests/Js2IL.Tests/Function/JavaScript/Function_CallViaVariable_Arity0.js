"use strict";

function getMessage() {
  return "Hello";
}

let f = getMessage;
console.log(f());
