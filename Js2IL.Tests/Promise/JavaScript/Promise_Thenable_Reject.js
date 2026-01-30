"use strict";\r\n\r\nfunction thenImpl(resolve, reject) {
  reject("err");
}

const thenable = { then: thenImpl };

Promise.resolve(thenable).catch(error => console.log(error));
