import * as Mitata from "mitata";
import process from "node:process";

export const bench = Mitata.bench;
export const group = Mitata.group;
export const summary = Mitata.summary;

/** @param {Parameters<typeof Mitata["run"]>["0"]} opts */
export function run(opts = {}) {
  if (process?.env?.BENCHMARK_RUNNER) {
    opts.format = "json";
  }

  return Mitata.run(opts);
}
