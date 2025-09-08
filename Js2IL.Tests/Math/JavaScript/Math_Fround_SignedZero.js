// Validate fround rounding and -0 preservation
const a = Math.fround(1.337);
const b = Math.fround(-0);
function toStr(n){
  return (n === 0 && (1/n) < 0) ? "-0" : ("" + n);
}
// For a, rely on default number formatting consistency across .NET invariant culture
console.log("" + a);
console.log(toStr(b));
