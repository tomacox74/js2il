"use strict";

var a = [1, 2, 3];

try {
  a.length = 3.5;
  console.log("no-throw");
} catch (e) {
  // Keep expectations stable across runtimes: just check the error name.
  console.log(e.name);
}

console.log(a.length);
