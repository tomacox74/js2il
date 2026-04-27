"use strict";

function LocalURL(base) {
    this.base = base;
}

LocalURL.prototype.resolve = function(href) {
    return this.base + "/" + href;
};

module.exports = LocalURL;
