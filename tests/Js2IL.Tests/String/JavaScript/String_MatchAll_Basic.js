"use strict";

const regexMatches = Array.from("test1 test22".matchAll(/t(e)(st\d+)/g));
console.log(regexMatches.length);
console.log(regexMatches[0][0]);
console.log(regexMatches[0][1]);
console.log(regexMatches[0][2]);
console.log(regexMatches[0].index);
console.log(regexMatches[1][0]);
console.log(regexMatches[1][1]);
console.log(regexMatches[1][2]);
console.log(regexMatches[1].index);

const literalMatches = Array.from("aba".matchAll("a"));
console.log(literalMatches.length);
console.log(literalMatches[0][0]);
console.log(literalMatches[1].index);

try {
  Array.from("aba".matchAll(/a/));
} catch (error) {
  console.log(error.name);
}
