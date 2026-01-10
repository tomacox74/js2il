function outer() {
  let x = 10;

  function inner(y) {
    return x + y;
  }

  let f = inner;
  console.log(f(1));

  x = 20;
  console.log(f(1));
}

outer();
