// Copyright (C) 2012 Ecma International. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.
/*---
esid: sec-json.stringify
description: >
  String space is used as gap.
info: |
  JSON.stringify ( value [ , replacer [ , space ] ] )

  [...]
  7. Else if Type(space) is String, then
    a. If the length of space is 10 or less, let gap be space; otherwise
    let gap be the String value consisting of the first 10 code units of space.
---*/

function assertSameValue(actual, expected, message) {
    var passed = Object.is(actual, expected);
    console.log(passed);
    if (!passed) {
        throw new Error(message || "Expected SameValue");
    }
}

var obj = {
    a1: {
        b1: [1, 2, 3, 4],
        b2: {
            c1: 1,
            c2: 2
        }
    },
    a2: "a2"
};

assertSameValue(JSON.stringify(obj, null, ""), JSON.stringify(obj));
assertSameValue(
    JSON.stringify(obj, null, "  "),
    [
        "{",
        '  "a1": {',
        '    "b1": [',
        "      1,",
        "      2,",
        "      3,",
        "      4",
        "    ],",
        '    "b2": {',
        '      "c1": 1,',
        '      "c2": 2',
        "    }",
        "  },",
        '  "a2": "a2"',
        "}"
    ].join("\n")
);
