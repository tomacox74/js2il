// Upstream: test/language/statements/class/subclass/derived-class-return-override-with-number.js
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
