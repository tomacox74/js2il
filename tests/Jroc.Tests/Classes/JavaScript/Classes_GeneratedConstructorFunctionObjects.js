"use strict";

class Base {
  constructor(value = 2) {
    this.value = value;
  }

  static label() {
    return "base";
  }
}

class Derived extends Base {
  constructor(value) {
    super(value + 1);
  }
}

console.log(typeof Base);
console.log(Base === Base);
console.log(Base.prototype.constructor === Base);
console.log(Object.getPrototypeOf(Derived) === Base);
console.log(Base.name);
console.log(Base.length);
console.log(Base.label());
console.log(new Base().value);
console.log(Reflect.construct(Derived, [4]).value);

class Alternate {}
const alternateInstance = Reflect.construct(Base, [9], Alternate);
console.log(Object.getPrototypeOf(alternateInstance) === Alternate.prototype);

try {
  Base();
} catch (error) {
  console.log(error.name);
}

const BoundBase = Base.bind(null, 7);
console.log(new BoundBase().value);

const ProxiedBase = new Proxy(Base, {});
console.log(new ProxiedBase(8).value);

class Renamed {
  static name = "renamed";
}
console.log(Renamed.name);

class NamedMethod {
  static name() {
    return "method";
  }
}
console.log(NamedMethod.name());

const Inferred = class {};
console.log(Inferred.name);

const First = class {};
const Second = class {};
console.log(First === Second);
