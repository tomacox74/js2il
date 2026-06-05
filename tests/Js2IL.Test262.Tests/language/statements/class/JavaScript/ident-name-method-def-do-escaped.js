function __test262SameValue(actual, expected) {
    return Object.is(actual, expected);
}

var assert = function assert(value, message) {
    var passed = !!value;
    console.log(passed);
    if (!passed) {
        throw new Error(message || "Assertion failed");
    }
};

assert.sameValue = function(actual, expected, message) {
    var passed = __test262SameValue(actual, expected);
    console.log(passed);
    if (!passed) {
        throw new Error(message || "Expected SameValue");
    }
};

// This file was procedurally generated from the following sources:
// - src/identifier-names/do-escaped.case
// - src/identifier-names/default/class-statement-method-def.template
/*---
description: do is a valid identifier name, using escape (MethodDefinition)
esid: prod-PropertyDefinition
features: [class]
flags: [generated]
info: |
    ObjectLiteral :
      { PropertyDefinitionList }
      { PropertyDefinitionList , }

    PropertyDefinitionList:
      PropertyDefinition
      PropertyDefinitionList , PropertyDefinition

    PropertyDefinition:
      MethodDefinition
      ...

    MethodDefinition:
      PropertyName ( UniqueFormalParameters ){ FunctionBody }

    PropertyName:
      LiteralPropertyName
      ...

    LiteralPropertyName:
      IdentifierName
      ...

    Reserved Words

    A reserved word is an IdentifierName that cannot be used as an Identifier.

---*/


class C {
  \u0064o() { return 42; }
}

var obj = new C();

assert.sameValue(obj['do'](), 42, 'property exists');
