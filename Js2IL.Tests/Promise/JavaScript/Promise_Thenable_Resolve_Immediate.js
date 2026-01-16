const thenable = {
  then: function(resolve, reject) {
    resolve(42);
  }
};

Promise.resolve(thenable).then(value => console.log(value));
