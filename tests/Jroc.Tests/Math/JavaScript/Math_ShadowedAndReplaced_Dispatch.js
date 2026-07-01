(function () {
  const Math = {
    abs: function () { return 77; },
    round: function () { return 88; },
    PI: 9
  };

  console.log(Math.abs(-1), Math.round(0), Math.PI);
})();

Math.abs = function () {
  return 123;
};

console.log(Math.abs(-1));
