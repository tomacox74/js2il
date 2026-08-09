try {
    forwardArrow();
    const forwardArrow = () => "wrong";
    console.log("NO_FORWARD_ARROW_TDZ");
} catch (error) {
    console.log("forwardArrow", error instanceof ReferenceError);
}

try {
    forwardExpression();
    const forwardExpression = function () {
        return "wrong";
    };
    console.log("NO_FORWARD_EXPRESSION_TDZ");
} catch (error) {
    console.log("forwardExpression", error instanceof ReferenceError);
}

try {
    callHoisted();
    const target = () => "wrong";

    function callHoisted() {
        return target();
    }

    console.log("NO_HOISTED_TDZ");
} catch (error) {
    console.log("hoisted", error instanceof ReferenceError);
}

switch (1) {
    case 0:
        const skipped = () => "wrong";
        break;
    case 1:
        try {
            skipped();
            console.log("NO_SWITCH_TDZ");
        } catch (error) {
            console.log("switch", error instanceof ReferenceError);
        }
        break;
}
