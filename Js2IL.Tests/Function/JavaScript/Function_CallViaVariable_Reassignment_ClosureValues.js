function makeAdder(n) {
  return function (x) {
    return n + x;
  };
}

let f = makeAdder(5);
console.log(f(1));

f = makeAdder(100);
console.log(f(1));
