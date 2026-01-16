const thenable = {
  then: function(resolve, reject) {
    resolve({
      then: function(resolve2, reject2) {
        resolve2(7);
      }
    });
  }
};

Promise.resolve(thenable).then(value => console.log(value));
