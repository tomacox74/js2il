function outer(p) {
  function inner() { console.log("param:", p); }
  inner();
}

outer(123);
