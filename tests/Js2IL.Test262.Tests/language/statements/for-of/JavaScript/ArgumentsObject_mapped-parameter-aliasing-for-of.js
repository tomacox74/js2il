"use strict";

const mapped = Function(
  "a",
  "b",
  "c",
  "let i = 0; for (const value of arguments) { console.log(value); a = b; b = c; c = i; i++; } console.log(i);");

mapped(1, 2, 3);
