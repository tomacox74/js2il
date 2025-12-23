const path = require('path');

function joinWrapper(a, b) {
  return path.join(a, b);
}

let joinedPath = joinWrapper('a','b').replace('\\', '/');

console.log(joinedPath);
