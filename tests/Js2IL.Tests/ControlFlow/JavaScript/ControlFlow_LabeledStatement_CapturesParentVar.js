"use strict";

function outer() {
  var NodeTraversal = {
    nextSkippingChildren: function(n) {
      return n + 1;
    }
  };

  return function nextNode() {
    var node = 0;

    CHILDREN:
    while (true) {
      node = NodeTraversal.nextSkippingChildren(node);
      console.log(node);
      break CHILDREN;
    }
  };
}

outer()();
