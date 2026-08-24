const VALUE = 1;

function receive(value) {
  return value;
}

function receiveIdentifier(value) {
  return value;
}

const makeReader = object => {
  with (object) {
    return () => receive(VALUE + 1);
  }
};

const makeIdentifierReader = object => {
  with (object) {
    return () => receiveIdentifier(VALUE);
  }
};

console.log(makeReader({ VALUE: "shadow" })());
console.log(makeReader({})());
console.log(makeIdentifierReader({ VALUE: "shadow" })());
console.log(makeIdentifierReader({})());

const NESTED_VALUE = 1;
const outer = { NESTED_VALUE: 2 };
with (outer) {
  with ({}) {
    ++NESTED_VALUE;
  }
}
console.log(outer.NESTED_VALUE);
