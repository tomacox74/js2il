var callable = {
  sum: function (a, b, c) {
    return a + b + c;
  }
};

function invokeCallable() {
  return callable.sum(1, 2, 3);
}

console.log(invokeCallable());
