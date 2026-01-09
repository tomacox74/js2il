let obj = { a: 1, b: 2, c: 3 };
for (let k in obj) {
  if (k === "b") {
    continue;
  }
  console.log(k);
}
