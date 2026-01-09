let x = 0;
function inc() {
  x = x + 1;
  return true;
}

// RHS should NOT execute
const y = true || inc();
console.log(x);
