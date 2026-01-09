let i = 0;

outer: do {
  i++;
  for (let j = 0; j < 1; j++) {
    console.log(i);
    if (i === 2) {
      break outer;
    }
  }
} while (i < 5);

console.log("done");
