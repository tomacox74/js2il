"use strict";

class BinaryReceiver {
  add(value) {
    return value;
  }

  multiply(value) {
    return value;
  }

  shift(value) {
    return value;
  }

  increment(value) {
    value++;
    return value;
  }

  incrementBoolean(value) {
    value++;
    return value;
  }

  dynamicAdd(value) {
    return value;
  }

  bigint(value) {
    return typeof value;
  }

  destructure(value) {
    value++;
    [value] = ["text"];
    return typeof value;
  }

  iterate(value) {
    value++;
    for (value of ["text"]) {
    }
    return typeof value;
  }

  iterateVar(value) {
    value++;
    for (var value of ["text"]) {
    }
    return typeof value;
  }

  iterateVarIn(value) {
    value++;
    for (var value in { text: true }) {
    }
    return typeof value;
  }

  redeclare(value) {
    var value = "text";
    value++;
    return typeof value;
  }

  functionRedeclare(value) {
    function value() {
    }
    value++;
    return typeof value;
  }

  run() {
    const number = 7;
    const text = "value";
    const bigA = BigInt(1);
    const bigB = BigInt(1);

    console.log(this.add(number + 1));
    console.log(this.multiply(number * 2));
    console.log(this.shift(number >>> 1));
    console.log(this.increment(number + 1));
    console.log(this.incrementBoolean(true));
    console.log(this.dynamicAdd(text + 1));
    console.log(this.destructure(number));
    console.log(this.iterate(number));
    console.log(this.iterateVar(number));
    console.log(this.iterateVarIn(number));
    console.log(this.redeclare(number));
    if (false) {
      this.bigint((bigA & bigB) | (bigA & bigB));
      this.functionRedeclare(number);
    }
  }
}

new BinaryReceiver().run();
