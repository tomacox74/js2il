function outer(callback) {
  (function inner() {
    for (const x of [1, 2, 3]) {
      callback(x);
    }
  })();
}

outer(function (v) {
  console.log(v);
});
