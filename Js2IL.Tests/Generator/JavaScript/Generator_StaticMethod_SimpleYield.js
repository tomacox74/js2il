class Gen {
  static *values() {
    yield 42;
    return 100;
  }
}

const it = Gen.values();

let r;
r = it.next();
console.log("s1:", r.value, "done:", r.done);

r = it.next();
console.log("s2:", r.value, "done:", r.done);
