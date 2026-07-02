// Upstream: test/language/statements/class/subclass/derived-class-return-override-with-number.js
function assert(value) { console.log(!!value); }
assert.throws = function(expectedCtor, fn) {
  try {
    fn();
    console.log(false);
  } catch (error) {
    console.log(error instanceof expectedCtor || error.constructor === expectedCtor || error.name === expectedCtor.name);
  }
};

class Base {
  constructor() {}
}

class Derived extends Base {
  constructor() {
    super();
    return 0;
  }
}

assert.throws(TypeError, function() {
  new Derived();
});
