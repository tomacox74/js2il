const thenable = {
  then: function(resolve, reject) {
    resolve(42);
  }
};

Promise.resolve(10).then(() => thenable).then(value => console.log(value));
