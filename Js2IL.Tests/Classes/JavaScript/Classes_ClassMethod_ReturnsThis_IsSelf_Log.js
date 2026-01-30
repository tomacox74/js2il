"use strict";\r\n\r\nclass Self {
  isSelf() {
    return this;
  }
}

const s = new Self();
console.log(!!s.isSelf());
