"use strict";

const match = /(?<word>a)(?<optional>b)?/d.exec("a");

console.log(Object.getPrototypeOf(match.groups) === null);
console.log(JSON.stringify(Object.getOwnPropertyDescriptor(match, "groups")));
console.log(JSON.stringify(Object.keys(match.groups)));
console.log(match.groups.word);
console.log(Object.hasOwn(match.groups, "optional"));
console.log(match.groups.optional === undefined);
console.log(JSON.stringify(Object.getOwnPropertyDescriptor(match.groups, "word")));
console.log(JSON.stringify(match.groups));

console.log(JSON.stringify(match.indices[0]));
console.log(JSON.stringify(match.indices[1]));
console.log(match.indices[2] === undefined);
console.log(Object.getPrototypeOf(match.indices.groups) === null);
console.log(JSON.stringify(Object.getOwnPropertyDescriptor(match.indices, "groups")));
console.log(JSON.stringify(Object.keys(match.indices.groups)));
console.log(JSON.stringify(match.indices.groups.word));
console.log(Object.hasOwn(match.indices.groups, "optional"));
console.log(match.indices.groups.optional === undefined);
console.log(match.groups !== match.indices.groups);

const stringMatch = "a".match(/(?<letter>a)/);
console.log(Object.getPrototypeOf(stringMatch.groups) === null);
console.log(stringMatch.groups.letter);

const matchAll = Array.from("a a".matchAll(/(?<letter>a)/g));
console.log(matchAll.length);
console.log(matchAll[0].groups.letter);
console.log(matchAll[1].groups.letter);

const unnamed = /a/d.exec("a");
console.log(unnamed.groups === undefined);
console.log(unnamed.indices.groups === undefined);
console.log(JSON.stringify(Object.getOwnPropertyDescriptor(unnamed, "groups")));
console.log(JSON.stringify(Object.getOwnPropertyDescriptor(unnamed.indices, "groups")));
