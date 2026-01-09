let i = 0;

outer: while (true) {
  for (let j = 0; j < 1; j++) {
    console.log("a");
    break outer;
  }

  // Should not execute
  console.log("x");
}

console.log("b");
