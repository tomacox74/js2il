Object.defineProperty(Math, "sqrt", {
  value: function () {
    return 789;
  },
  configurable: true,
  writable: true
});

console.log(Math.sqrt(9));
