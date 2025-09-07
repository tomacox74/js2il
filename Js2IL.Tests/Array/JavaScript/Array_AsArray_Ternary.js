// Confirms ternary using Array.isArray result
function asArray(x) {
  return Array.isArray(x) ? x : [];
}

const a = asArray([1]);
const b = asArray(0);
console.log(a.length);
console.log(b.length);
