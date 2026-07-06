var arr = /(A)\1/.exec("AA");

if ((arr === null) || (arr[0] !== "AA")) {
  throw new Test262Error('#1: var arr = (/(A)\\1/.exec("AA")); arr[0] === "AA". Actual. ' + (arr && arr[0]));
}

if ((arr === null) || (arr[1] !== "A")) {
  throw new Test262Error('#2: var arr = (/(A)\\1/.exec("AA")); arr[1] === "A". Actual. ' + (arr && arr[1]));
}
