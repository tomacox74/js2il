"use strict";

function MyEvent(type) {
    this.type = type;
    this.timeStamp = Date.now();
}

MyEvent.prototype = Object.create(Object.prototype, {
    dumpToConsole: {
        value: function dumpToConsole() {
            console.log(this.type);
        }
    }
});

var e = new MyEvent("click");
e.dumpToConsole();
