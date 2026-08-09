"use strict";

const dns = require("node:dns");

console.log("same module:", dns === require("dns"));
console.log("default order:", dns.getDefaultResultOrder());
dns.setDefaultResultOrder("ipv4first");
console.log("changed order:", dns.getDefaultResultOrder());

let synchronous = true;
dns.lookup("localhost", { all: true, family: 4, order: "ipv4first" }, (error, addresses) => {
    console.log("async:", !synchronous);
    console.log("error:", error === null);
    console.log("addresses:", Array.isArray(addresses), addresses.length > 0);
    console.log("family:", addresses[0].family);
});
synchronous = false;

try {
    dns.lookup("localhost", { all: "true" }, () => {});
} catch (error) {
    console.log("invalid all:", error instanceof TypeError);
}
