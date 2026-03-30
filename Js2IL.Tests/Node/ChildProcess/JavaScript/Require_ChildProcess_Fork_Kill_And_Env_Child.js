"use strict";

setTimeout(() => {
    process.send({
        env: process.env.JS2IL_KILL_MARKER,
        argv2: process.argv[2]
    });
}, 100);

setInterval(() => {}, 1000);
