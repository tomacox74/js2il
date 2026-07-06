if (parseFloat("Infinity") !== Number.POSITIVE_INFINITY) {
  throw new Test262Error('#1: parseFloat("Infinity") === Number.POSITIVE_INFINITY. Actual: ' + (parseFloat("Infinity")));
}

//CHECK#2
if (parseFloat("+Infinity") !== Number.POSITIVE_INFINITY) {
  throw new Test262Error('#2: parseFloat("+Infinity") === Number.POSITIVE_INFINITY. Actual: ' + (parseFloat("+Infinity")));
}

//CHECK#3
if (parseFloat("-Infinity") !== Number.NEGATIVE_INFINITY) {
  throw new Test262Error('#3: parseFloat("-Infinity") === Number.NEGATIVE_INFINITY. Actual: ' + (parseFloat("-Infinity")));
}


console.log(true);
