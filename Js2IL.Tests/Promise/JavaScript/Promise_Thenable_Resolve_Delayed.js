const thenable = {
  then: function(resolve, reject) {
    setTimeout(() => resolve(42), 10);
  }
};

Promise.resolve(thenable).then(value => console.log(value));
