"use strict";\r\n\r\nconst obj = JSON.parse('{"a":1,"b":true,"c":null}');
console.log(typeof obj, obj.a, obj.b, obj.c === null);
