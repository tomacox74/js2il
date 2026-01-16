const err = Error("boom");
console.log(err.name + ":" + err.message);
const typeErr = TypeError("oops");
console.log(typeErr.name + ":" + typeErr.message);
