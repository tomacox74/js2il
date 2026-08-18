try {
  String({
    toString: function () { return {}; },
    valueOf: function () { return {}; }
  });
} catch (error) {
  console.log(error.name);
}
