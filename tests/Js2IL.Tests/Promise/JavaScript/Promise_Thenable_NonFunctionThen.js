"use strict";

const obj = { then: 123 };

Promise.resolve(obj).then(value => console.log(typeof value.then));
