const regexp = /a/g;
const constructor = {};
constructor[Symbol.species] = function () {
  return {
    exec: function () {
      return { 0: Symbol("match") };
    }
  };
};
regexp.constructor = constructor;

try {
  "a".matchAll(regexp).next();
} catch (error) {
  console.log(error.name);
}
