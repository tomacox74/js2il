"use strict";

const diagnostics = require("node:diagnostics_channel");
console.log("same module:", diagnostics === require("diagnostics_channel"));

const channel = diagnostics.channel("jroc:diagnostics");
console.log("same channel:", channel === diagnostics.channel("jroc:diagnostics"));
console.log("initial subscribers:", channel.hasSubscribers);

const messages = [];
function listener(message, name) {
    messages.push(message + ":" + name);
}

diagnostics.subscribe("jroc:diagnostics", listener);
console.log("subscribed:", diagnostics.hasSubscribers("jroc:diagnostics"));
channel.publish("published");
console.log("synchronous:", messages.join(","));
console.log("removed:", diagnostics.unsubscribe("jroc:diagnostics", listener));
console.log("final subscribers:", channel.hasSubscribers);

const guarded = diagnostics.channel("undici:request:pending-requests");
if (guarded.hasSubscribers) {
    guarded.publish({ size: 1 });
}
console.log("guarded publish:", guarded.hasSubscribers);
