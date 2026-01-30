"use strict";\r\n\r\nconst arr = [1, 5, 10, 20];

const found = arr.find(function (x) {
  return x > 5;
});
console.log(found);

const notFound = arr.find(function (x) {
  return x > 100;
});
console.log(notFound);
