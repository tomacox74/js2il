"use strict";

function Base() {}
Base.prototype.inherited = "proto";

const value = Object.create(Base.prototype);
value.own = "yes";
value.default = "ignored";

module.exports = value;
