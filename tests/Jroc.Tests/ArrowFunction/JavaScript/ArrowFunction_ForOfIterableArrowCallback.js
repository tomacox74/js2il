const stripUnderscores = data => {
    for (const key of Object.keys(data).filter(k => /^_/.test(k))) {
        delete data[key];
    }

    return Object.keys(data).join(",");
};

console.log(stripUnderscores({ _name: "skip", run: "keep", _meta: "skip" }));
