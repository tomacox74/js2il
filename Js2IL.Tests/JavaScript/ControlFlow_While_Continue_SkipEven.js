let i = 1;
while (i <= 5) {
  if (i % 2 == 0) {
    i++;
    continue;
  }
  console.log("index is", i);
  i++;
}
