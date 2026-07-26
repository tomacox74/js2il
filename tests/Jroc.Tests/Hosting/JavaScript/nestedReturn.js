"use strict";

class Window {
  constructor() {
    this.document = {
      title: "Hello"
    };
  }

  get title() {
    return this.document.title;
  }

  setTitle(title) {
    this.document.title = title;
    return this.document.title;
  }

  fail() {
    throw new Error("nested boom");
  }
}

const windowValue = new Window();

exports.getWindow = function () {
  return windowValue;
};

exports.getTitleViaHost = function () {
  // Useful for sanity: this should work in-script too.
  return exports.getWindow().document.title;
};

exports.getTitle = function (win) {
  return win.document.title;
};

exports.getHostValue = function (win) {
  return win.hostValue;
};
