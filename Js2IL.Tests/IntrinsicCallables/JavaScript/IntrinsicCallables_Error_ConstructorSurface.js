"use strict";

const ErrorCtor = Error;
const err = ErrorCtor("boom");
console.log(err.name + ":" + err.message);
console.log(Error.isError(err));
console.log(Error.isError({}));
console.log(Error.prototype.constructor === Error);
console.log(Error.prototype.name + ":" + Error.prototype.message);
console.log(Error.prototype.toString.call({ name: "X", message: "Y" }));
console.log(Error.prototype.toString.call({ message: "Y" }));
console.log(Error.prototype.toString.call({ name: "X" }));
console.log(Error.prototype.toString.call(err));
