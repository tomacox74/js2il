const path = require('path');

function outer() {
  function inner(a, b, c) {
    // Call into the Node 'path' module from a nested function
    return path.join(a, b, c);
  }
  const result = inner('a', 'b', 'c.txt');
  console.log(result);
}

outer();
