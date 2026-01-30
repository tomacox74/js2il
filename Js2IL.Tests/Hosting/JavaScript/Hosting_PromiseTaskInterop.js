"use strict";\r\n\r\nfunction immediateResolve(value) {
    return Promise.resolve(value);
}

function immediateReject(reason) {
    return Promise.reject(reason);
}

function immediateRejectError() {
    return Promise.reject(new Error("boom"));
}

function timeoutResolve(ms, value) {
    return new Promise((resolve) => {
        setTimeout(() => resolve(value), ms);
    });
}

function timeoutReject(ms, reason) {
    return new Promise((_, reject) => {
        setTimeout(() => reject(reason), ms);
    });
}

module.exports = {
    immediateResolve,
    immediateReject,
    immediateRejectError,
    timeoutResolve,
    timeoutReject
};
