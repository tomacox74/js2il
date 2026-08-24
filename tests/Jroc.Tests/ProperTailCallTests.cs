namespace Jroc.Tests;

public sealed class ProperTailCallTests
{
    [Fact]
    public void DynamicTailCalls_ResolveBeforeDirectCallerUsesResult()
    {
        var result = InMemoryTestCompiler.CompileAndExecute(
            "proper-tail-call-direct-caller",
            "ProperTailCalls",
            _ => ("""
            "use strict";

            function getFinish() {
              return finish;
            }

            function finish(n) {
              if (n === 0) {
                return 41;
              }
              return getFinish()(n - 1);
            }

            function wrapper() {
              return finish(100000) + 1;
            }

            console.log(wrapper());
            """, null));

        Assert.Equal($"42{Environment.NewLine}", result.Output);
    }

    [Fact]
    public void MemberAndRestTailCalls_PreserveThisArgumentsAndClosures()
    {
        var result = InMemoryTestCompiler.CompileAndExecute(
            "proper-tail-call-observables",
            "ProperTailCalls",
            _ => ("""
            "use strict";

            const offset = 2;
            const receiver = {
              value: 0,
              recurse(n, ...values) {
                if (n === 0) {
                  return this.value + values[0] + offset;
                }
                this.value += 1;
                return this.recurse(n - 1, ...values);
              }
            };

            console.log(receiver.recurse(100000, 3));
            """, null));

        Assert.Equal($"100005{Environment.NewLine}", result.Output);
    }

    [Fact]
    public void ClassMethodTailCalls_ResolveAcrossTypedCallBoundaries()
    {
        var result = InMemoryTestCompiler.CompileAndExecute(
            "proper-tail-call-class-method",
            "ProperTailCalls",
            _ => ("""
            "use strict";

            class Counter {
              constructor() {
                this.value = 0;
              }

              recurse(n) {
                if (n === 0) {
                  return this.value;
                }
                this.value += 1;
                return this.recurse(n - 1);
              }
            }

            const counter = new Counter();
            console.log(counter.recurse(100000) + 1);
            """, null));

        Assert.Equal($"100001{Environment.NewLine}", result.Output);
    }

    [Fact]
    public void CallsProtectedByCatchOrFinally_RemainInTheCaller()
    {
        var result = InMemoryTestCompiler.CompileAndExecute(
            "proper-tail-call-protected-regions",
            "ProperTailCalls",
            _ => ("""
            "use strict";

            function caughtInCaller() {
              try {
                return null();
              } catch (error) {
                return error.name;
              }
            }

            const order = [];
            function target() {
              order.push("target");
              return 1;
            }
            function finalizedInCaller() {
              try {
                return target();
              } finally {
                order.push("finally");
              }
            }

            console.log(caughtInCaller());
            console.log(finalizedInCaller());
            console.log(order.join(","));
            """, null));

        Assert.Equal(
            $"TypeError{Environment.NewLine}1{Environment.NewLine}target,finally{Environment.NewLine}",
            result.Output);
    }

    [Fact]
    public void CallsInForOfBodies_RunBeforeIteratorClose()
    {
        var result = InMemoryTestCompiler.CompileAndExecute(
            "proper-tail-call-iterator-close",
            "ProperTailCalls",
            _ => ("""
            "use strict";

            const events = [];
            function iterable() {
              return {
                [Symbol.iterator]() {
                  return {
                    next() {
                      return { value: 1, done: false };
                    },
                    return() {
                      events.push("close");
                      return { done: true };
                    }
                  };
                }
              };
            }

            function valueTarget(value) {
              events.push("value");
              return value;
            }
            function returnValue() {
              for (const value of iterable()) {
                return valueTarget(value);
              }
            }

            console.log(returnValue());
            console.log(events.join(","));

            events.length = 0;
            function throwTarget() {
              events.push("throw");
              throw new Error("boom");
            }
            function returnThrow() {
              for (const value of iterable()) {
                return throwTarget();
              }
            }

            try {
              returnThrow();
            } catch (error) {
              events.push("caught");
            }
            console.log(events.join(","));
            """, null));

        Assert.Equal(
            $"1{Environment.NewLine}value,close{Environment.NewLine}throw,close,caught{Environment.NewLine}",
            result.Output);
    }

    [Fact]
    public void MemberTaggedTemplateTailCalls_PreserveReceiver()
    {
        var result = InMemoryTestCompiler.CompileAndExecute(
            "proper-tail-call-member-tag",
            "ProperTailCalls",
            _ => ("""
            "use strict";

            const receiver = {
              count: 0,
              tag(strings, n) {
                if (n === 0) {
                  return this.count;
                }
                this.count += 1;
                return this.tag`${n - 1}`;
              }
            };

            function run(n) {
              return receiver.tag`${n}`;
            }

            console.log(run(100000));
            """, null));

        Assert.Equal($"100000{Environment.NewLine}", result.Output);
    }

    [Fact]
    public void SuperMemberTailCalls_AreStackSafe()
    {
        var result = InMemoryTestCompiler.CompileAndExecute(
            "proper-tail-call-super-member",
            "ProperTailCalls",
            _ => ("""
            "use strict";

            class Base {
              recurse(n) {
                if (n === 0) {
                  return 1;
                }
                return this.recurse(n - 1);
              }
            }

            class Derived extends Base {
              recurse(n) {
                if (n === 0) {
                  return 1;
                }
                return super.recurse(n - 1);
              }
            }

            console.log(new Derived().recurse(100000));
            """, null));

        Assert.Equal($"1{Environment.NewLine}", result.Output);
    }
}
