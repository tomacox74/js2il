"use strict";

for (const input of ["A", "AAA", "??", "Zm9v?"]) {
  try {
    Uint8Array.fromBase64(input);
    console.log("no error");
  } catch (error) {
    console.log(error && error.name === "SyntaxError");
    console.log(typeof error.message === "string" && error.message.length > 0);
  }
}
