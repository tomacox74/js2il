let arr = ["a", "b", "c"]; 

outer: for (const x of arr) {
  for (let j = 0; j < 1; j++) {
    if (x === "b") {
      continue outer;
    }
    console.log(x);
  }
}
