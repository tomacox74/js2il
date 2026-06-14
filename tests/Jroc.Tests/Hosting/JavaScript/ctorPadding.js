"use strict";

class C {
  constructor(x) {
    this.x = x;
  }
}

exports.undefinedWhenMissingArgs = function () {
  // Forces dynamic-new-on-value lowering: constructor is held in a variable.
  // Missing JS args should become undefined, not a host MissingMethodException.
  let K = C;
  let o = new K();
  return o.x === undefined;
};
