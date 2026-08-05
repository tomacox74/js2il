"use strict";

Promise.prototype.then = () => console.log("prototype override");
import("node:path").then(() => console.log("direct"));
