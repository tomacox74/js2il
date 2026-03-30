"use strict";

var arr = ['a', 'bb', 'ccc'];
var lengths = arr.map(function(x) { return x.length; });
for (var i = 0; i < lengths.length; i++) {
  console.log(lengths[i]);
}
