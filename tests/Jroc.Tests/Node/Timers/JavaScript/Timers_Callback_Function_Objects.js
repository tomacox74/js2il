"use strict";

setImmediate(function* generatorImmediate() {
  yield "unused";
});

setImmediate(function ordinaryImmediate() {
  console.log("generator:accepted");
});

setTimeout(async function asyncTimeout(value) {
  console.log("async:" + value);
}, 0, "timer");

try {
  setTimeout(42, 0);
} catch (error) {
  console.log("error:" + error.message);
}
