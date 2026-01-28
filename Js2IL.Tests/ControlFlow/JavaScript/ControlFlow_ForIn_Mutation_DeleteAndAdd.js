const obj = { a: 1, b: 2, c: 3 };

for (const k in obj) {
  console.log(k);
  if (k === 'a') {
    delete obj.b;
    obj.d = 4;
  }
}
