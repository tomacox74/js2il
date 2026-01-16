const thenable = {
  then: function(resolve, reject) {
    reject("err");
  }
};

Promise.resolve(thenable).catch(error => console.log(error));
