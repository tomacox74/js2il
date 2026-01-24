let state = 0;

function startTimer(ms) {
    setTimeout(() => {
        state = 123;
    }, ms);
}

function getState() {
    return state;
}

module.exports = {
    startTimer,
    getState
};
