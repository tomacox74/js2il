copied from https://github.com/mozilla-it/krakenbenchmark.mozilla.org/tree/master/tests/kraken-1.1

`json-parse-financial.js` and `json-parse-financial-data.js` come from
[revision 77ef4e08af23c131166762adad8cb460c49160e8](https://github.com/mozilla-it/krakenbenchmark.mozilla.org/tree/77ef4e08af23c131166762adad8cb460c49160e8/tests/kraken-1.1).
The 5,534-byte JSON payload is unchanged; the data script's constant
concatenation is flattened into one string literal to avoid expensive compiler
traversal during setup. Its SHA-256 is
`94998a8cd5712e05b23d4745037119085dc0cc60d0cb2460715ae7fffba6b5bf`.
The workload's original 1,000-iteration loop is
registered through `runTest(callback, 1000)` so data loading and script
initialization remain outside the timed region. Each callback parses the entire
financial JSON payload and stores the result in `x`, as in the upstream workload.
The scenario is enabled by default and discovered automatically.

