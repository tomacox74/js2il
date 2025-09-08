const path = require('path');

function joinWrapper(a, b) {
  return path.join(a, b);
}

console.log(joinWrapper('a','b'));
