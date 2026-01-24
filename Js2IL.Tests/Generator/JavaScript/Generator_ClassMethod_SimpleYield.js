class Gen {
  *values() {
    yield 1;
    yield 2;
    return 3;
  }
}

const g = new Gen();
const it = g.values();

let r;
r = it.next();
console.log("v1:", r.value, "done:", r.done);

r = it.next();
console.log("v2:", r.value, "done:", r.done);

r = it.next();
console.log("v3:", r.value, "done:", r.done);
