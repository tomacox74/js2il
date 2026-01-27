const fns = [];

for (let i = 0; i < 3; i++) {
  fns.push(() => i);
  if (i === 1) {
    continue;
  }
}

console.log(fns[0]());
console.log(fns[1]());
console.log(fns[2]());
