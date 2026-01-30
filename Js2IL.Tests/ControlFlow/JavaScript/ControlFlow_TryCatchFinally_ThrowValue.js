"use strict";

console.log("a");

try {
  throw 123;
} catch (e) {
  console.log(e);
} finally {
  console.log("c");
}
