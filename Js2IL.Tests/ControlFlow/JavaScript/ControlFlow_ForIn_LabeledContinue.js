let obj = { a: 1, b: 2, c: 3 };

outer: for (let k in obj) {
  for (let j = 0; j < 1; j++) {
    if (k === "b") {
      continue outer;
    }
    console.log(k);
  }
}
