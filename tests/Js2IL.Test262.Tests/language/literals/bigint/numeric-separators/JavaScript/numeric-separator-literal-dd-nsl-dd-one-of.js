// Copyright (C) 2019 Leo Balter. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
esid: prod-NumericLiteralSeparator
description: NonZeroDigit NumericLiteralSeparator DecimalDigit
info: |
  NumericLiteral ::
    DecimalIntegerLiteral BigIntLiteralSuffix
    NumericLiteralBase BigIntLiteralSuffix

  NumericLiteralBase ::
    BinaryIntegerLiteral
    OctalIntegerLiteral
    HexIntegerLiteral

  BigIntLiteralSuffix :: n

  NumericLiteralSeparator ::
    _

  DecimalIntegerLiteral ::
    ...
    NonZeroDigit NumericLiteralSeparator_opt DecimalDigits


  DecimalDigits ::
    DecimalDigit
    ...

  DecimalDigit :: one of
    0 1 2 3 4 5 6 7 8 9

features: [BigInt, numeric-separator-literal]
---*/

console.log(Object.is(1_0n, 10n));
console.log(Object.is(1_1n, 11n));
console.log(Object.is(1_2n, 12n));
console.log(Object.is(1_3n, 13n));
console.log(Object.is(1_4n, 14n));
console.log(Object.is(1_5n, 15n));
console.log(Object.is(1_6n, 16n));
console.log(Object.is(1_7n, 17n));
console.log(Object.is(1_8n, 18n));
console.log(Object.is(1_9n, 19n));
