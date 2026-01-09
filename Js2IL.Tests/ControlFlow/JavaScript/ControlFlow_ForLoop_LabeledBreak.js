outer: for (let i = 0; i < 3; i++) {
  for (let j = 0; j < 1; j++) {
    console.log(i);
    if (i === 1) {
      break outer;
    }
  }
}
