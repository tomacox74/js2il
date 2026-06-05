const opts = {
    format: "mitata",
    filter: null
};

opts.throw ??= false;
opts.filter ??= "all";
opts.format ??= "other";

console.log(opts.throw);
console.log(opts.filter);
console.log(opts.format);
