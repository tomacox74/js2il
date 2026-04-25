"use strict";

var calls = 0;
var usurper = {};
var lexicalThis = this;

[1].forEach(value => {
    calls++;
    console.log(this === lexicalThis);
    console.log(this === usurper);
}, usurper);

console.log(calls);
