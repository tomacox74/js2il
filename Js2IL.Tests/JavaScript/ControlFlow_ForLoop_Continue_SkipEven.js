// Print odd numbers between 1 and 5 using continue to skip evens
var i = 0;
for (i = 1; i <= 5; i = i + 1) {
  if ((i % 2) == 0) {
    continue;
  }
  console.log("index is", i);
}
