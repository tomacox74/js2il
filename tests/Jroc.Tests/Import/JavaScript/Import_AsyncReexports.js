import defer * as deferred from "./Import_AsyncReexports_Leaf.mjs";
import { value, increment } from "./Import_AsyncReexports_Lib.mjs";
import * as ns from "./Import_AsyncReexports_Lib.mjs";
increment();
console.log(value, ns.value, ns.own, Object.keys(ns).join(","));
