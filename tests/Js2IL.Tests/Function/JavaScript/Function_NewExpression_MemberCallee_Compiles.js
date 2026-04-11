"use strict";

var impl = {
  Window: function (d) {
    // Avoid relying on `this` semantics for the test.
    // Returning an object should override the constructed instance per JS `new` rules.
    return { d: d };
  }
};

var createWindow = function (x) {
  var w = new impl.Window(x);
  return w.d;
};

console.log(createWindow(123));
