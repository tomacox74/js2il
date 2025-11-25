using System;
using System.IO;
using System.Linq;
using System.Reflection;

class Program
{
    static int Main(string[] args)
    {
        var cwd = AppContext.BaseDirectory;
        var candidates = new[] {
            Path.GetFullPath(Path.Combine("..","..","..","out_prime_run","PrimeJavaScript.dll")),
            Path.GetFullPath(Path.Combine("..","..","..","out_prime","PrimeJavaScript.dll")),
            Path.GetFullPath(Path.Combine("..","..","..","out_prime","PrimeJavaScript.dll")),
            Path.GetFullPath(Path.Combine("out_prime_run","PrimeJavaScript.dll")),
            Path.GetFullPath(Path.Combine("out_prime","PrimeJavaScript.dll")),
            Path.GetFullPath("PrimeJavaScript.dll")
        };

        string? asmPath = candidates.FirstOrDefault(p => File.Exists(p));
        Console.WriteLine("Inspector working directory: " + cwd);
        if (asmPath == null)
        {
            Console.WriteLine("Could not find PrimeJavaScript.dll in expected locations. Checked:");
            foreach (var c in candidates) Console.WriteLine("  " + c);
            return 2;
        }

        Console.WriteLine("Found assembly: " + asmPath);
        try
        {
            var asm = Assembly.LoadFrom(asmPath);
            var t = asm.GetType("Classes.BitArray");
            if (t == null)
            {
                Console.WriteLine("Type Classes.BitArray not found in assembly. Types present (first 50):");
                foreach (var tt in asm.GetTypes().Take(50)) Console.WriteLine("  " + tt.FullName);
                return 3;
            }

            Console.WriteLine("Found type: " + t.FullName);

            // Try to find a constructor that takes 1 parameter or any constructor
            ConstructorInfo? ctor = t.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .OrderBy(c => c.GetParameters().Length)
                .FirstOrDefault();
            if (ctor == null)
            {
                Console.WriteLine("No constructor found on Classes.BitArray.");
                return 4;
            }

            Console.WriteLine("Using constructor with " + ctor.GetParameters().Length + " parameters.");
            // Build arguments: use 100 for numeric param, null for others
            object?[] ctorArgs = ctor.GetParameters().Select((p, i) => (object?) (p.ParameterType == typeof(object) || p.ParameterType == typeof(System.Object) ? (object)100 : null)).ToArray();

            object? instance = null;
            try
            {
                instance = ctor.Invoke(ctorArgs);
            }
            catch (TargetInvocationException tie)
            {
                Console.WriteLine("Constructor threw: " + tie.InnerException?.ToString());
                Console.WriteLine("Full exception: " + tie);
                return 5;
            }

            if (instance == null)
            {
                Console.WriteLine("Constructor returned null (unexpected for instance constructor).");
                return 6;
            }

            Console.WriteLine("Instance created: " + instance.GetType().FullName);
            // List all instance fields and their values
            Console.WriteLine();
            Console.WriteLine("--- Instance fields and values ---");
            var allFields = t.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var f in allFields)
            {
                object? fv = null;
                try { fv = f.GetValue(instance); } catch (Exception ex) { fv = "<get failed: " + ex.GetType().Name + ">"; }
                Console.WriteLine($"{f.Name} (Public={f.IsPublic}) = {(fv == null ? "<null>" : fv is string ? fv : fv.GetType().FullName)}");
            }

            // Look for field 'wordArray' (public or non-public)
            var field = t.GetField("wordArray", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (field == null)
            {
                Console.WriteLine("Field 'wordArray' not found on Classes.BitArray. Listing instance fields:");
                foreach (var f in t.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                {
                    Console.WriteLine($"  {f.Name} (Public={f.IsPublic}, Private={f.IsPrivate})");
                }
                return 7;
            }

            var val = field.GetValue(instance);
            Console.WriteLine($"wordArray field value: {(val == null ? "<null>" : val.GetType().FullName)}");

            if (val != null)
            {
                // Try to read a property 'length' or 'Length' or field 'length'
                var vt = val.GetType();
                var prop = vt.GetProperty("length", BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase) ?? vt.GetProperty("Length", BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (prop != null)
                {
                    try { var pval = prop.GetValue(val); Console.WriteLine($"wordArray.{prop.Name} = {pval}"); } catch { }
                }
                else
                {
                    var f2 = vt.GetField("length", BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase) ?? vt.GetField("Length", BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                    if (f2 != null) Console.WriteLine($"wordArray.{f2.Name} = {f2.GetValue(val)}");
                }
            }

            Console.WriteLine();
            Console.WriteLine("--- Methods on Classes.BitArray ---");
            var methods = t.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            foreach (var m in methods.OrderBy(m => m.Name))
            {
                Console.WriteLine($"{m.Name} (Static={m.IsStatic}) -> ({string.Join(", ", m.GetParameters().Select(p => p.ParameterType.Name + " " + p.Name))})");
            }

            // Try to find setBitsTrue
            var targetMethod = methods.FirstOrDefault(m => string.Equals(m.Name, "setBitsTrue", StringComparison.OrdinalIgnoreCase));
            if (targetMethod == null)
            {
                Console.WriteLine("Method setBitsTrue not found on Classes.BitArray.");
                return 0;
            }

            Console.WriteLine($"\nFound method: {targetMethod.Name}, Static={targetMethod.IsStatic}. Attempting invocation tests...");
            try
            {
                // If instance method, invoke on instance with two numeric args if possible
                if (!targetMethod.IsStatic)
                {
                    var parms = targetMethod.GetParameters();
                    var argList = new object?[parms.Length];
                    for (int i = 0; i < parms.Length; i++) argList[i] = 1; // use 1 for numeric params
                    Console.WriteLine("Invoking as instance method with args: " + string.Join(",", argList.Select(a => a?.ToString() ?? "null")));
                    var r = targetMethod.Invoke(instance, argList);
                    Console.WriteLine("Invocation succeeded; return value: " + (r ?? "<null>"));
                }
                else
                {
                    // static method: try passing instance as first arg, then numeric args
                    var parms = targetMethod.GetParameters();
                    var argList = new object?[parms.Length];
                    if (parms.Length > 0) argList[0] = instance; // try instance as first param if method is essentially a JS-style function taking this
                    for (int i = 1; i < parms.Length; i++) argList[i] = 1;
                    Console.WriteLine("Invoking as static method with args: " + string.Join(",", argList.Select(a => a?.ToString() ?? "null")));
                    var r = targetMethod.Invoke(null, argList);
                    Console.WriteLine("Invocation succeeded; return value: " + (r ?? "<null>"));
                }
            }
            catch (TargetInvocationException tie)
            {
                Console.WriteLine("Invocation threw: " + tie.InnerException?.ToString());
            }

            // Try multiple argument combinations to see if some inputs avoid the NRE
            Console.WriteLine("\nTrying multiple argument combinations for setBitsTrue:");
            var testArgs = new object?[][] {
                new object?[] { 0, 0, 0 },
                new object?[] { 0, 1, 1 },
                new object?[] { 1, 2, 2 },
                new object?[] { 0, 10, 10 },
                new object?[] { 2, 3, 3 }
            };
            foreach (var ta in testArgs)
            {
                try
                {
                    Console.WriteLine("Invoking setBitsTrue(" + string.Join(",", ta) + ")");
                    var r = targetMethod.Invoke(instance, ta);
                    Console.WriteLine("  -> Success, return: " + (r ?? "<null>"));
                }
                catch (TargetInvocationException tie)
                {
                    Console.WriteLine("  -> Threw: " + tie.InnerException?.GetType().Name + ": " + tie.InnerException?.Message);
                }
            }

            // Inspect IL for field and method references to see what setBitsTrue touches
            try
            {
                var mb = targetMethod.GetMethodBody();
                var il = mb?.GetILAsByteArray();
                if (il != null && il.Length > 0)
                {
                    Console.WriteLine("\n--- IL references (fields/methods) in setBitsTrue ---");
                    var module = asm.ManifestModule;
                    for (int i = 0; i < il.Length; i++)
                    {
                        byte op = il[i];
                        // look for opcodes that embed metadata tokens (little-endian 4-byte)
                        if (op == 0x7B || op == 0x7C || op == 0x7D || op == 0x7E || op == 0x80) // ldfld, ldflda, stfld, ldsfld, stsfld
                        {
                            if (i + 4 >= il.Length) break;
                            int token = BitConverter.ToInt32(il, i + 1);
                            try
                            {
                                var f = module.ResolveField(token);
                                Console.WriteLine($"IL opcode 0x{op:X2} at {i}: field -> {f.DeclaringType?.FullName}.{f.Name}");
                            }
                            catch (Exception ex) { Console.WriteLine($"IL opcode 0x{op:X2} at {i}: could not resolve field token {token} ({ex.Message})"); }
                            i += 4;
                        }
                        else if (op == 0x28 || op == 0x6F) // call, callvirt
                        {
                            if (i + 4 >= il.Length) continue;
                            int token = BitConverter.ToInt32(il, i + 1);
                            try
                            {
                                var mi = module.ResolveMethod(token);
                                Console.WriteLine($"IL opcode 0x{op:X2} at {i}: method -> {mi.DeclaringType?.FullName}.{mi.Name}");
                            }
                            catch (Exception ex) { Console.WriteLine($"IL opcode 0x{op:X2} at {i}: could not resolve method token {token} ({ex.Message})"); }
                            i += 4;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("IL inspection failed: " + ex);
            }

            // Also try setBitTrue and testBitTrue helpers if present
            foreach (var name in new[] { "setBitTrue", "testBitTrue" })
            {
                var m = methods.FirstOrDefault(mm => string.Equals(mm.Name, name, StringComparison.OrdinalIgnoreCase));
                if (m == null) { Console.WriteLine($"Method {name} not found."); continue; }
                Console.WriteLine($"\nInvoking {name} (Static={m.IsStatic})");
                try
                {
                    if (!m.IsStatic)
                    {
                        var pcount = m.GetParameters().Length;
                        var invokeArgs = Enumerable.Repeat<object?>(1, pcount).ToArray();
                        var r = m.Invoke(instance, invokeArgs);
                        Console.WriteLine($"{name} returned: " + (r ?? "<null>"));
                    }
                    else
                    {
                        var pcount = m.GetParameters().Length;
                        var invokeArgs = new object?[pcount];
                        if (pcount > 0) invokeArgs[0] = instance;
                        for (int i = 1; i < pcount; i++) invokeArgs[i] = 1;
                        var r = m.Invoke(null, invokeArgs);
                        Console.WriteLine($"{name} returned: " + (r ?? "<null>"));
                    }
                }
                catch (TargetInvocationException tie)
                {
                    Console.WriteLine($"{name} threw: " + tie.InnerException?.ToString());
                }
            }

            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Inspector exception: " + ex);
            return 1;
        }
    }
}
