let i = 0;

outer: while (true) {
  for (let j = 0; j < 2; j++) {
    if (j === 1) {
      console.log("a");
      break outer;
    }
  }

  // Should not execute
  console.log("x");
}

console.log("b");
