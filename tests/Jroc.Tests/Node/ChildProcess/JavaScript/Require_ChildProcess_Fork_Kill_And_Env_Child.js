"use strict";

setTimeout(() => {
    process.send({
        env: process.env.JROC_KILL_MARKER,
        argv2: process.argv[2]
    });
}, 100);

setInterval(() => {}, 1000);
