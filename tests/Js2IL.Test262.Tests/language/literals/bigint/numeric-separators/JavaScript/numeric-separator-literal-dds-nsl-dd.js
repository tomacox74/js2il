// Copyright (C) 2019 Leo Balter. All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
esid: prod-NumericLiteralSeparator
description: DecimalDigits NumericLiteralSeparator DecimalDigit
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

  DecimalDigits ::
    ...
    DecimalDigits NumericLiteralSeparator DecimalDigit

features: [BigInt, numeric-separator-literal]
---*/

console.log(Object.is(123456789_0n, 1234567890n));
console.log(Object.is(123456789_1n, 1234567891n));
console.log(Object.is(123456789_2n, 1234567892n));
console.log(Object.is(123456789_3n, 1234567893n));
console.log(Object.is(123456789_4n, 1234567894n));
console.log(Object.is(123456789_5n, 1234567895n));
console.log(Object.is(123456789_6n, 1234567896n));
console.log(Object.is(123456789_7n, 1234567897n));
console.log(Object.is(123456789_8n, 1234567898n));
console.log(Object.is(123456789_9n, 1234567899n));
