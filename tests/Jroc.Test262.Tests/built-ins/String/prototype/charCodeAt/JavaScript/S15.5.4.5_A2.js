function __FACTORY() {};

__FACTORY.prototype.charCodeAt = String.prototype.charCodeAt;

var __instance = new __FACTORY;

assert.sameValue(__instance.charCodeAt(-1), NaN);
