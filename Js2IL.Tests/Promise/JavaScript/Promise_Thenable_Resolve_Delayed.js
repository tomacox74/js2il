"use strict";\r\n\r\nfunction thenImpl(resolve, reject) {
  function later() {
    resolve(42);
  }
  setTimeout(later, 10);
}

const thenable = { then: thenImpl };

Promise.resolve(thenable).then(value => console.log(value));
