"use strict";

function outer(arr) {
  function inner(cb) {
    // Call map on the param inside a nested function to force runtime member dispatch
    return arr.map(cb);
  }
  // map to element lengths, verifying delegate wiring (value => value.length)
  return inner((x) => x.length);
}

const result = outer(["a", "bb", "ccc"]);
for (let i = 0; i < result.length; i++) {
  console.log(result[i]);
}
