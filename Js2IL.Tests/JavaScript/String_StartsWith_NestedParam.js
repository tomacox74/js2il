function outer(s) {
  function inner(prefix) {
    // Call startsWith on the param inside a nested function to force runtime member dispatch
    return s.startsWith(prefix);
  }
  return inner("He");
}

console.log(outer("Hello, world!"));
