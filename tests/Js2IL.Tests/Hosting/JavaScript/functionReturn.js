"use strict";

exports.getIncrementer = function () {
  return function (x) {
    return x + 1;
  };
};
