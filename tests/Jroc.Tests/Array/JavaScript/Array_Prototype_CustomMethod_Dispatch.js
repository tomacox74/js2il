"use strict";

Array.prototype.removeGraphNode = function (obj) {
    var index = this.indexOf(obj);
    if (index !== -1) {
        this.splice(index, 1);
    }

    return this.join(",");
};

var openList = [1, 2, 3];
console.log(openList.removeGraphNode(2));
console.log(openList.join(","));
