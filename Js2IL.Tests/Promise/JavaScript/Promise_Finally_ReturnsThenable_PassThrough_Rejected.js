"use strict";\r\n\r\nfunction thenImpl(resolve, reject) {
  resolve(999);
}

const thenable = { then: thenImpl };

Promise.reject("orig").finally(() => thenable).catch(e => console.log(e));
