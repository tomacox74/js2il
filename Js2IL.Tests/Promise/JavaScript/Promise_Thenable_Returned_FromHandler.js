function thenImpl(resolve, reject) {
  resolve(42);
}

const thenable = { then: thenImpl };

Promise.resolve(10).then(() => thenable).then(value => console.log(value));
