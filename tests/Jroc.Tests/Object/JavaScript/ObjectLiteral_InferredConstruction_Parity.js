const eligible = {
  text: "hello",
  n: 42,
  flag: true,
  boxed: null,
  fn: function () {
    return "fn";
  }
};

console.log(eligible.text, eligible.n, eligible.flag, eligible.boxed === null, typeof eligible.fn);

const a = {
  text: "hello",
  n: 42,
  flag: true,
  boxed: null,
  fn: function () {
    return "fn";
  }
};

console.log(a.text, a.n, a.flag, a.boxed === null);
console.log(a.fn());
console.log(Object.keys(a).join(","));
console.log(JSON.stringify(a));

const descriptor = Object.getOwnPropertyDescriptor(a, "n");
console.log(descriptor.value, descriptor.writable, descriptor.enumerable, descriptor.configurable);

const unsafe = { x: 1 };
leak(unsafe);
console.log(unsafe.x);

function leak(value) {
  console.log(Object.keys(value).join(","));
}
