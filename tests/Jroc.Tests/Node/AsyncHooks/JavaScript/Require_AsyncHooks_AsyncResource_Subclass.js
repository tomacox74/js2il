"use strict";

const { AsyncResource } = require("node:async_hooks");

class UndiciStyleHandler extends AsyncResource {
    constructor(callback) {
        super("UNDICI_REQUEST");
        this.callback = callback;
    }

    deliver(error, data) {
        return this.runInAsyncScope(this.callback, this, error, data);
    }
}

const handler = new UndiciStyleHandler(function (error, data) {
    console.log("derived receiver:", this === handler);
    console.log("error:", error);
    return data.statusCode;
});

console.log("derived:", handler instanceof UndiciStyleHandler);
console.log("base:", handler instanceof AsyncResource);
console.log("status:", handler.deliver(null, { statusCode: 200 }));
