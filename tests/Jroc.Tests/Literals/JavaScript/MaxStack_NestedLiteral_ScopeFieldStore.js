"use strict";

// Regression: storing a deeply nested array/object literal into a scope field
// pushes the scope instance before inline literal construction. The maxstack
// estimator must account for the extra receiver slot (InvalidProgramException).
var g1 = [[{ "x": 0 }]];

function go() {
  return g1;
}

console.log(go()[0][0].x);
