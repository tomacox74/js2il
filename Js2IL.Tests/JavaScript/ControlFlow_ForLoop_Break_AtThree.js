// Print numbers starting at 1, break at 3 so only 1 and 2 print
for (let i = 1; i <= 5; i = i + 1) {
  if (i == 3) {
    break;
  }
  console.log("index is", i);
}
