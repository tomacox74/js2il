// Repro: member call on non-identifier receiver
const s = (String('a|b|c')).replace(/\|/g, '\\|');
console.log(s);
