// Print numbers starting at 1, break at 3 so only 1 and 2 print
let i = 1;
while (i <= 5) {
  if (i == 3) {
    break;
  }
  console.log("index is", i);
  i = i + 1;
}
