"use strict";

const holder = {
    target: function propertyTarget(value) {
        if (value === 0) return propertyTarget(1);
        return value;
    }
};
console.log(holder.target("text"));

function factory() {
    return function returnedTarget(value) {
        if (value === 0) return returnedTarget(1);
        return value;
    };
}
console.log(factory()("text"));

function invoke(callback) {
    return callback("text");
}
console.log(invoke(function callbackTarget(value) {
    if (value === 0) return callbackTarget(1);
    return value;
}));

const targets = [
    function arrayTarget(value) {
        if (value === 0) return arrayTarget(1);
        return value;
    }
];
console.log(targets[0]("text"));

let assigned;
assigned = function assignedTarget(value) {
    if (value === 0) return assignedTarget(1);
    return value;
};
console.log(assigned("text"));
