"use strict";

var DocumentLike = {
  _preremoveNodeIterators: {
    value: function (toBeRemoved) {
      if (this._nodeIterators) {
        this._nodeIterators.forEach(function (ni) {
          ni._preremove(toBeRemoved);
        });
      }
    }
  }
};

var iterator = {
  _preremove: function (v) {
    console.log(v);
  }
};

var host = {
  _nodeIterators: [iterator]
};

DocumentLike._preremoveNodeIterators.value.call(host, 42);
