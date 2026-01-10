function add1(x) {
  return x + 1;
}

function add2(x) {
  return x + 2;
}

let f = add1;
console.log(f(10));

f = add2;
console.log(f(10));
