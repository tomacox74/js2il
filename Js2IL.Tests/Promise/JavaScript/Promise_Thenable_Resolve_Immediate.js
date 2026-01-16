function thenImpl(resolve, reject) {
  resolve(42);
}

const thenable = { then: thenImpl };

Promise.resolve(thenable).then(value => console.log(value));
