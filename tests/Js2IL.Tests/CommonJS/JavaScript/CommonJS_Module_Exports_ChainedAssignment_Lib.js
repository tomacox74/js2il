"use strict";

// Issue #558 repro: chained assignment where the assignment-expression result is used.
// Node/CommonJS pattern: set module.exports and exports to the same new object.

exports = module.exports = {
  answer: 42,
  greet: function (name) {
    return "hello " + name;
  }
};
