"use strict";\r\n\r\nfunction thenImpl(resolve, reject) {
  resolve(999);
}

const thenable = { then: thenImpl };

Promise.resolve(42).finally(() => thenable).then(v => console.log(v));
