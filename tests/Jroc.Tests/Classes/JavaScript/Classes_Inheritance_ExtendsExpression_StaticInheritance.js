function getBase() {
  return Uint8Array;
}

function create() {
  return class Derived extends getBase() {
  };
}

const first = create();
const second = create();

console.log(first === second);
console.log(Object.getPrototypeOf(first) === Uint8Array);
console.log(Object.prototype.hasOwnProperty.call(first, "fromHex"));
console.log(first.fromHex("ff").length);
