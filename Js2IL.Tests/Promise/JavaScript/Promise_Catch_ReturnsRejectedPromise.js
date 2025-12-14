Promise.reject("err1").catch(e => Promise.reject("err2")).catch(e => console.log("Caught:", e));
