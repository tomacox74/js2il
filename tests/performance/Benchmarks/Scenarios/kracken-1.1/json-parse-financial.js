// Adapted from Kraken 1.1 json-parse-financial.js.
// Source: mozilla-it/krakenbenchmark.mozilla.org@77ef4e08af23c131166762adad8cb460c49160e8
var x;

var parseFinancial = function() {
  x = JSON.parse(data);
};

runTest(parseFinancial, 1000);
