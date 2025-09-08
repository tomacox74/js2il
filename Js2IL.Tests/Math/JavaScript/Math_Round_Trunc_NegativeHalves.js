// Check rounding near halves and truncation, including negative zero preservation
const nums = [0.4, 0.5, (0 - 0.4), (0 - 0.5), (0 - 0.50001), (0 - 0.49999), 1.5, (0 - 1.5)];
const r = nums.map(n => Math.round(n));
const t = nums.map(n => Math.trunc(n));
function toStr(n){
  return (n === 0 && (1/n) < 0) ? "-0" : ("" + n);
}
console.log(r.map(toStr).join(" "));
console.log(t.map(toStr).join(" "));
