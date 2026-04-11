"use strict";

class Greeter {
  _secret = "TopSecret"; // private-like instance field
  logSecret() {
    console.log(this._secret);
  }
}

const g = new Greeter();
g.logSecret();
