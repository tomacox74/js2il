import * as lib from "./Import_TypedReexports_Lib.mjs";
import { hidden } from "./Import_TypedReexports_Source.mjs";
console.log(hidden, lib.text === "export const hidden = 7;\n", Object.keys(lib).join(","), lib.bytes.default instanceof Uint8Array);
