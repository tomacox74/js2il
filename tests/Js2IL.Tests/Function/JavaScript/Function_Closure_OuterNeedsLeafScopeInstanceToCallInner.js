"use strict";

let x = 42;

function outer() {
  // outer does NOT reference x, so it has no captured fields.
  // inner references a global, but not outer variables.
  // With generalized scopes layout, inner still expects outer's scope instance in the scopes chain.
  function inner() {
    console.log(x);
  }

  inner();
}

outer();
