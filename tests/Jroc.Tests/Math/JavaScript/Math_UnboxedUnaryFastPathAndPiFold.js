const value = -1.5;
const absValue = Math.abs(value);
const roundValue = Math.round(value);
const sqrtValue = Math.sqrt(9);
const sinScaled = Math.round(Math.sin(Math.PI / 2) * 1000000);
const cosScaled = Math.round(Math.cos(Math.PI) * 1000000);
const degToRadScaled = Math.round((Math.PI / 180) * 100000000);

console.log(
  absValue,
  roundValue,
  sqrtValue,
  sinScaled,
  cosScaled,
  degToRadScaled
);
