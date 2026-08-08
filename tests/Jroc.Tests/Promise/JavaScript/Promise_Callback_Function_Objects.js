"use strict";

Promise.resolve(2)
  .then(function ordinaryReaction(value) {
    return value + 1;
  })
  .then(async function asyncReaction(value) {
    return value + 1;
  })
  .then(function* generatorReaction(value) {
    yield value + 1;
  })
  .then(function consumeGenerator(iterator) {
    console.log("value:" + iterator.next().value);
    console.log("done:" + iterator.next().done);
  });

Promise.try(function promiseTryCallback(left, right) {
  return left + right;
}, 2, 3).then(function (value) {
  console.log("try:" + value);
});

try {
  new Promise(42);
} catch (error) {
  console.log("error:" + error.message);
}
