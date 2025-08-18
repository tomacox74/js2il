let i = 1;
do {
  if (i % 2 == 0) {
    i++;
    continue;
  }
  console.log("index is", i);
  i++;
} while (i <= 5);
