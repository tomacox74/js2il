// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: When "break" is evaluated, (break, empty, empty) is returned
es5id: 12.8_A3
description: Using "break" without Identifier within labeled loop
---*/

LABEL_OUT : var x=0, y=0;

LABEL_DO_LOOP : do {
    LABEL_IN : x=2;
    break ;
    LABEL_IN_2 : var y=2;
    
    function IN_DO_FUNC(){}
} while(0);

LABEL_ANOTHER_LOOP : do {
    ;
} while(0);

function OUT_FUNC(){}

//////////////////////////////////////////////////////////////////////////////
//CHECK#1
console.log((x === 2) && (y === 0));
//
//////////////////////////////////////////////////////////////////////////////
