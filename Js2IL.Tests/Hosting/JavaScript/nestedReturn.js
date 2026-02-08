"use strict";

exports.getWindow = function () {
  return {
    document: {
      title: "Hello"
    }
  };
};

exports.getTitleViaHost = function () {
  // Useful for sanity: this should work in-script too.
  return exports.getWindow().document.title;
};
