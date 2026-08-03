"use strict";

const path = require("path");

import("node:path").then(namespace => {
    namespace.default.join = (...parts) => `dynamic:${parts.join("|")}`;
    console.log(path.join("a", "b"));
});
