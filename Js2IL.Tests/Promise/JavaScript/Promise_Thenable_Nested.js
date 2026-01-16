function innerThen(resolve2, reject2) {
  resolve2(7);
}

const innerThenable = { then: innerThen };

function outerThen(resolve, reject) {
  resolve(innerThenable);
}

const thenable = { then: outerThen };

Promise.resolve(thenable).then(value => console.log(value));
