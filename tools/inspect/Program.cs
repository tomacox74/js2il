using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices;

class Program
{
    static int Main(string[] args)
    {
        if (args.Length >= 1 && (string.Equals(args[0], "--metadata-members", StringComparison.OrdinalIgnoreCase) ||
                                 string.Equals(args[0], "--members", StringComparison.OrdinalIgnoreCase)))
        {
            return ListMetadataReaderNestedMembers();
        }

        if (args.Length >= 1 && string.Equals(args[0], "--members-all", StringComparison.OrdinalIgnoreCase))
        {
            return ListMetadataReaderMembersAll();
        }

        if (args.Length >= 2 && (string.Equals(args[0], "--nested", StringComparison.OrdinalIgnoreCase) ||
                                 string.Equals(args[0], "--dump-nested", StringComparison.OrdinalIgnoreCase)))
        {
            return DumpNestedClassTable(args[1]);
        }

        if (args.Length >= 2 && string.Equals(args[0], "--nested-raw", StringComparison.OrdinalIgnoreCase))
        {
            return DumpNestedClassTableRaw(args[1]);
        }

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

    private static int DumpNestedClassTable(string assemblyPath)
    {
        if (!File.Exists(assemblyPath))
        {
            Console.WriteLine("File not found: " + assemblyPath);
            return 2;
        }

        Console.WriteLine("Reading metadata from: " + assemblyPath);

        using var stream = File.OpenRead(assemblyPath);
        using var peReader = new PEReader(stream);
        if (!peReader.HasMetadata)
        {
            Console.WriteLine("PE file has no metadata.");
            return 3;
        }

        var reader = peReader.GetMetadataReader();

        static string GetTypeName(MetadataReader r, TypeDefinitionHandle h)
        {
            if (h.IsNil) return "<nil>";
            var td = r.GetTypeDefinition(h);
            var ns = r.GetString(td.Namespace);
            var name = r.GetString(td.Name);
            return string.IsNullOrEmpty(ns) ? name : ns + "." + name;
        }

        // Validate nesting via declaring-type lookup. This is also what the CLR relies on;
        // if the NestedClass table is missing or unsorted, GetDeclaringType will return nil.
        var nestedTypes = new List<(TypeDefinitionHandle Nested, TypeDefinitionHandle Enclosing)>();
        var missing = new List<TypeDefinitionHandle>();

        foreach (var tdHandle in reader.TypeDefinitions)
        {
            var td = reader.GetTypeDefinition(tdHandle);
            var vis = td.Attributes & TypeAttributes.VisibilityMask;
            var isNestedVisibility = vis is TypeAttributes.NestedPublic or TypeAttributes.NestedPrivate or TypeAttributes.NestedFamily or
                                     TypeAttributes.NestedAssembly or TypeAttributes.NestedFamANDAssem or TypeAttributes.NestedFamORAssem;
            if (!isNestedVisibility)
            {
                continue;
            }

            var enclosing = td.GetDeclaringType();
            if (enclosing.IsNil)
            {
                missing.Add(tdHandle);
            }
            else
            {
                nestedTypes.Add((tdHandle, enclosing));
            }
        }

        foreach (var (nested, enclosing) in nestedTypes.OrderBy(p => MetadataTokens.GetRowNumber(p.Nested)))
        {
            Console.WriteLine($"nested={GetTypeName(reader, nested)} (0x{MetadataTokens.GetToken(nested):X8})  enclosing={GetTypeName(reader, enclosing)} (0x{MetadataTokens.GetToken(enclosing):X8})");
        }

        Console.WriteLine();
        Console.WriteLine($"Nested visibility types: {nestedTypes.Count + missing.Count}");
        Console.WriteLine($"Resolved declaring type: {nestedTypes.Count}");
        Console.WriteLine($"Missing declaring type: {missing.Count}");

        if (missing.Count > 0)
        {
            Console.WriteLine();
            Console.WriteLine("Types with nested visibility but no declaring type:");
            foreach (var h in missing.OrderBy(h => MetadataTokens.GetRowNumber(h)))
            {
                Console.WriteLine($"  {GetTypeName(reader, h)} (0x{MetadataTokens.GetToken(h):X8})");
            }
            return 1;
        }

        return 0;
    }

    private static int DumpNestedClassTableRaw(string assemblyPath)
    {
        if (!File.Exists(assemblyPath))
        {
            Console.WriteLine("File not found: " + assemblyPath);
            return 2;
        }

        using var stream = File.OpenRead(assemblyPath);
        using var peReader = new PEReader(stream);
        if (!peReader.HasMetadata)
        {
            Console.WriteLine("PE file has no metadata.");
            return 3;
        }

        var reader = peReader.GetMetadataReader();
        var metadata = new byte[reader.MetadataLength];
        unsafe
        {
            new ReadOnlySpan<byte>(reader.MetadataPointer, reader.MetadataLength).CopyTo(metadata);
        }

        // Parse metadata root to find #~ (or #-) stream.
        var tablesStream = TryGetMetadataStream(metadata, "#~") ?? TryGetMetadataStream(metadata, "#-");
        if (tablesStream == null)
        {
            Console.WriteLine("Could not find #~ or #- metadata tables stream.");
            return 4;
        }

        var stringsStream = TryGetMetadataStream(metadata, "#Strings");

        var (tablesOffset, tablesSize) = tablesStream.Value;
        var tables = metadata.AsSpan(tablesOffset, tablesSize);

        ReadOnlySpan<byte> strings = default;
        if (stringsStream != null)
        {
            var (strOffset, strSize) = stringsStream.Value;
            strings = metadata.AsSpan(strOffset, strSize);
        }

        // Parse #~ header
        if (tables.Length < 24)
        {
            Console.WriteLine("Tables stream too small.");
            return 5;
        }

        int offset = 0;
        offset += 4; // Reserved
        byte major = tables[offset++];
        byte minor = tables[offset++];
        byte heapSizes = tables[offset++];
        offset++; // Reserved2

        ulong validMask = ReadUInt64(tables, ref offset);
        ulong sortedMask = ReadUInt64(tables, ref offset);

        var rowCounts = new uint[64];
        int presentTables = 0;
        for (int i = 0; i < 64; i++)
        {
            if (((validMask >> i) & 1UL) == 0)
            {
                continue;
            }
            presentTables++;
            rowCounts[i] = ReadUInt32(tables, ref offset);
        }

        int stringIndexSize = (heapSizes & 0x01) != 0 ? 4 : 2;
        int guidIndexSize = (heapSizes & 0x02) != 0 ? 4 : 2;
        int blobIndexSize = (heapSizes & 0x04) != 0 ? 4 : 2;

        int SimpleIndexSize(TableIndex table) => rowCounts[(int)table] < 0x10000 ? 2 : 4;
        int CodedIndexSize(int tagBits, params TableIndex[] tablesForCode)
        {
            uint max = 0;
            foreach (var t in tablesForCode)
            {
                max = Math.Max(max, rowCounts[(int)t]);
            }
            // If max fits in 16-tagBits bits, index is 2 bytes; otherwise 4.
            return max < (1u << (16 - tagBits)) ? 2 : 4;
        }

        int ResolutionScopeSize() => CodedIndexSize(2, TableIndex.Module, TableIndex.ModuleRef, TableIndex.AssemblyRef, TableIndex.TypeRef);
        int TypeDefOrRefSize() => CodedIndexSize(2, TableIndex.TypeDef, TableIndex.TypeRef, TableIndex.TypeSpec);
        int MemberRefParentSize() => CodedIndexSize(3, TableIndex.TypeDef, TableIndex.TypeRef, TableIndex.ModuleRef, TableIndex.MethodDef, TableIndex.TypeSpec);
        int HasConstantSize() => CodedIndexSize(2, TableIndex.Field, TableIndex.Param, TableIndex.Property);
        int HasCustomAttributeSize() => CodedIndexSize(5,
            TableIndex.MethodDef,
            TableIndex.Field,
            TableIndex.TypeRef,
            TableIndex.TypeDef,
            TableIndex.Param,
            TableIndex.InterfaceImpl,
            TableIndex.MemberRef,
            TableIndex.Module,
            TableIndex.DeclSecurity,
            TableIndex.Property,
            TableIndex.Event,
            TableIndex.StandAloneSig,
            TableIndex.ModuleRef,
            TableIndex.TypeSpec,
            TableIndex.Assembly,
            TableIndex.AssemblyRef,
            TableIndex.File,
            TableIndex.ExportedType,
            TableIndex.ManifestResource,
            TableIndex.GenericParam,
            TableIndex.MethodSpec,
            TableIndex.GenericParamConstraint);
        int CustomAttributeTypeSize() => CodedIndexSize(3, TableIndex.MethodDef, TableIndex.MemberRef);
        int HasFieldMarshalSize() => CodedIndexSize(1, TableIndex.Field, TableIndex.Param);
        int HasDeclSecuritySize() => CodedIndexSize(2, TableIndex.TypeDef, TableIndex.MethodDef, TableIndex.Assembly);
        int HasSemanticsSize() => CodedIndexSize(1, TableIndex.Event, TableIndex.Property);
        int MethodDefOrRefSize() => CodedIndexSize(1, TableIndex.MethodDef, TableIndex.MemberRef);
        int MemberForwardedSize() => CodedIndexSize(1, TableIndex.Field, TableIndex.MethodDef);
        int ImplementationSize() => CodedIndexSize(2, TableIndex.File, TableIndex.AssemblyRef, TableIndex.ExportedType);

        int RowSize(TableIndex table)
        {
            return table switch
            {
                TableIndex.Module => 2 + stringIndexSize + guidIndexSize + guidIndexSize + guidIndexSize,
                TableIndex.TypeRef => ResolutionScopeSize() + stringIndexSize + stringIndexSize,
                TableIndex.TypeDef => 4 + stringIndexSize + stringIndexSize + TypeDefOrRefSize() + SimpleIndexSize(TableIndex.Field) + SimpleIndexSize(TableIndex.MethodDef),
                TableIndex.FieldPtr => SimpleIndexSize(TableIndex.Field),
                TableIndex.Field => 2 + stringIndexSize + blobIndexSize,
                TableIndex.MethodPtr => SimpleIndexSize(TableIndex.MethodDef),
                TableIndex.MethodDef => 4 + 2 + 2 + stringIndexSize + blobIndexSize + SimpleIndexSize(TableIndex.Param),
                TableIndex.ParamPtr => SimpleIndexSize(TableIndex.Param),
                TableIndex.Param => 2 + 2 + stringIndexSize,
                TableIndex.InterfaceImpl => SimpleIndexSize(TableIndex.TypeDef) + TypeDefOrRefSize(),
                TableIndex.MemberRef => MemberRefParentSize() + stringIndexSize + blobIndexSize,
                TableIndex.Constant => 2 + HasConstantSize() + blobIndexSize,
                TableIndex.CustomAttribute => HasCustomAttributeSize() + CustomAttributeTypeSize() + blobIndexSize,
                TableIndex.FieldMarshal => HasFieldMarshalSize() + blobIndexSize,
                TableIndex.DeclSecurity => 2 + HasDeclSecuritySize() + blobIndexSize,
                TableIndex.ClassLayout => 2 + 4 + SimpleIndexSize(TableIndex.TypeDef),
                TableIndex.FieldLayout => 4 + SimpleIndexSize(TableIndex.Field),
                TableIndex.StandAloneSig => blobIndexSize,
                TableIndex.EventMap => SimpleIndexSize(TableIndex.TypeDef) + SimpleIndexSize(TableIndex.Event),
                TableIndex.EventPtr => SimpleIndexSize(TableIndex.Event),
                TableIndex.Event => 2 + stringIndexSize + TypeDefOrRefSize(),
                TableIndex.PropertyMap => SimpleIndexSize(TableIndex.TypeDef) + SimpleIndexSize(TableIndex.Property),
                TableIndex.PropertyPtr => SimpleIndexSize(TableIndex.Property),
                TableIndex.Property => 2 + stringIndexSize + blobIndexSize,
                TableIndex.MethodSemantics => 2 + SimpleIndexSize(TableIndex.MethodDef) + HasSemanticsSize(),
                TableIndex.MethodImpl => SimpleIndexSize(TableIndex.TypeDef) + MethodDefOrRefSize() + MethodDefOrRefSize(),
                TableIndex.ModuleRef => stringIndexSize,
                TableIndex.TypeSpec => blobIndexSize,
                TableIndex.ImplMap => 2 + MemberForwardedSize() + stringIndexSize + SimpleIndexSize(TableIndex.ModuleRef),
                TableIndex.FieldRva => 4 + SimpleIndexSize(TableIndex.Field),
                TableIndex.EncLog => 4 + 4,
                TableIndex.EncMap => 4,
                TableIndex.Assembly => 4 + 2 + 2 + 2 + 2 + 4 + blobIndexSize + stringIndexSize + stringIndexSize,
                TableIndex.AssemblyProcessor => 4,
                TableIndex.AssemblyOS => 4 + 4 + 4,
                TableIndex.AssemblyRef => 2 + 2 + 2 + 2 + 4 + blobIndexSize + stringIndexSize + stringIndexSize + blobIndexSize,
                TableIndex.AssemblyRefProcessor => 4 + SimpleIndexSize(TableIndex.AssemblyRef),
                TableIndex.AssemblyRefOS => 4 + 4 + 4 + SimpleIndexSize(TableIndex.AssemblyRef),
                TableIndex.File => 4 + stringIndexSize + blobIndexSize,
                TableIndex.ExportedType => 4 + 4 + stringIndexSize + stringIndexSize + ImplementationSize(),
                TableIndex.ManifestResource => 4 + 4 + stringIndexSize + ImplementationSize(),
                TableIndex.NestedClass => SimpleIndexSize(TableIndex.TypeDef) + SimpleIndexSize(TableIndex.TypeDef),
                _ => throw new NotSupportedException($"Row size not implemented for table {table}")
            };
        }

        // Compute start of tables data
        int tablesDataStart = offset;

        int TableStart(TableIndex target)
        {
            int c = tablesDataStart;
            for (int ti = 0; ti < (int)target; ti++)
            {
                if (((validMask >> ti) & 1UL) == 0) continue;
                uint count = rowCounts[ti];
                if (count == 0) continue;
                c += checked((int)count * RowSize((TableIndex)ti));
            }
            return c;
        }

        // Walk tables in order to locate NestedClass table data.
        int cursor = TableStart(TableIndex.NestedClass);

        if (((validMask >> (int)TableIndex.NestedClass) & 1UL) == 0)
        {
            Console.WriteLine("NestedClass table not present in metadata.");
            return 6;
        }

        uint nestedCount = rowCounts[(int)TableIndex.NestedClass];
        int typeDefIndexSize = SimpleIndexSize(TableIndex.TypeDef);
        int nestedRowSize = RowSize(TableIndex.NestedClass);

        Console.WriteLine($"Tables stream version: {major}.{minor}, heapSizes=0x{heapSizes:X2}");
        Console.WriteLine($"Valid mask: 0x{validMask:X16}");
        Console.WriteLine($"Sorted mask: 0x{sortedMask:X16}");
        Console.WriteLine($"NestedClass rows: {nestedCount}, TypeDef index size: {typeDefIndexSize} bytes");

        int start = cursor;
        int end = start + checked((int)nestedCount * nestedRowSize);
        if (end > tables.Length)
        {
            Console.WriteLine("NestedClass table extends beyond tables stream.");
            return 7;
        }

        uint lastNested = 0;
        bool sortedByNested = true;

        for (int i = 0; i < nestedCount; i++)
        {
            int rowOff = start + (i * nestedRowSize);
            uint nested = typeDefIndexSize == 2
                ? ReadUInt16(tables, rowOff)
                : ReadUInt32(tables, rowOff);
            uint enclosing = typeDefIndexSize == 2
                ? ReadUInt16(tables, rowOff + typeDefIndexSize)
                : ReadUInt32(tables, rowOff + typeDefIndexSize);

            if (i > 0 && nested < lastNested)
            {
                sortedByNested = false;
            }
            lastNested = nested;

            if (i < 16)
            {
                Console.WriteLine($"{i + 1,4}: nested=0x0200{nested:X4} enclosing=0x0200{enclosing:X4}");
            }
        }

        Console.WriteLine($"NestedClass sorted by nested TypeDef index: {sortedByNested}");

        // Dump TypeDef flags/name/namespace for the Scope type (row 2) and module type (row 7) if present.
        if (((validMask >> (int)TableIndex.TypeDef) & 1UL) != 0 && stringsStream != null)
        {
            int typeDefStart = TableStart(TableIndex.TypeDef);
            int typeDefRowSize = RowSize(TableIndex.TypeDef);
            Console.WriteLine();

            int typeDefOrRefSize = TypeDefOrRefSize();
            int fieldIndexSize = SimpleIndexSize(TableIndex.Field);
            int methodIndexSize = SimpleIndexSize(TableIndex.MethodDef);

            foreach (var row in new uint[] { 2, 5, 7 })
            {
                if (row == 0) continue;
                if (row > rowCounts[(int)TableIndex.TypeDef])
                {
                    Console.WriteLine($"TypeDef row {row} not present (TypeDef count={rowCounts[(int)TableIndex.TypeDef]}).");
                    continue;
                }

                int rOff = typeDefStart + checked((int)(row - 1) * typeDefRowSize);
                uint flags = ReadUInt32(tables, rOff);
                int idxOff = rOff + 4;
                uint nameIx = stringIndexSize == 2 ? ReadUInt16(tables, idxOff) : ReadUInt32(tables, idxOff);
                idxOff += stringIndexSize;
                uint nsIx = stringIndexSize == 2 ? ReadUInt16(tables, idxOff) : ReadUInt32(tables, idxOff);
                idxOff += stringIndexSize;

                uint extendsRaw = typeDefOrRefSize == 2 ? ReadUInt16(tables, idxOff) : ReadUInt32(tables, idxOff);
                idxOff += typeDefOrRefSize;
                uint fieldList = fieldIndexSize == 2 ? ReadUInt16(tables, idxOff) : ReadUInt32(tables, idxOff);
                idxOff += fieldIndexSize;
                uint methodList = methodIndexSize == 2 ? ReadUInt16(tables, idxOff) : ReadUInt32(tables, idxOff);

                string name = ReadString(strings, nameIx);
                string ns = ReadString(strings, nsIx);

                Console.WriteLine($"TypeDef 0x0200{row:X4}: flags=0x{flags:X8} name='{name}' ns='{ns}' extendsRaw=0x{extendsRaw:X} fieldList={fieldList} methodList={methodList}");
            }
        }

        return sortedByNested ? 0 : 1;
    }

    private static (int Offset, int Size)? TryGetMetadataStream(byte[] metadataRoot, string streamName)
    {
        var span = metadataRoot.AsSpan();
        if (span.Length < 16) return null;

        // Metadata root signature "BSJB" (0x424A5342)
        uint sig = ReadUInt32(span, 0);
        if (sig != 0x424A5342) return null;

        int offset = 0;
        offset += 4; // signature
        offset += 2; // major
        offset += 2; // minor
        offset += 4; // reserved
        uint versionLen = ReadUInt32(span, offset);
        offset += 4;
        offset += checked((int)versionLen);
        offset = Align4(offset);
        offset += 2; // flags
        ushort streams = ReadUInt16(span, offset);
        offset += 2;

        for (int i = 0; i < streams; i++)
        {
            int streamOffset = checked((int)ReadUInt32(span, offset));
            offset += 4;
            int streamSize = checked((int)ReadUInt32(span, offset));
            offset += 4;

            // Read null-terminated name, padded to 4 bytes
            int nameStart = offset;
            int nameEnd = nameStart;
            while (nameEnd < span.Length && span[nameEnd] != 0)
            {
                nameEnd++;
            }
            if (nameEnd >= span.Length) return null;
            var name = System.Text.Encoding.ASCII.GetString(span.Slice(nameStart, nameEnd - nameStart));
            offset = Align4(nameEnd + 1);

            if (string.Equals(name, streamName, StringComparison.Ordinal))
            {
                return (streamOffset, streamSize);
            }
        }

        return null;
    }

    private static int Align4(int value) => (value + 3) & ~3;

    private static string ReadString(ReadOnlySpan<byte> stringsHeap, uint index)
    {
        if (stringsHeap.IsEmpty) return "";
        if (index == 0) return "";
        if (index >= stringsHeap.Length) return $"<bad-string-index:{index}>";
        int i = (int)index;
        int end = i;
        while (end < stringsHeap.Length && stringsHeap[end] != 0)
        {
            end++;
        }
        return System.Text.Encoding.UTF8.GetString(stringsHeap.Slice(i, end - i));
    }

    private static ushort ReadUInt16(ReadOnlySpan<byte> span, int offset)
        => (ushort)(span[offset] | (span[offset + 1] << 8));

    private static uint ReadUInt32(ReadOnlySpan<byte> span, int offset)
        => (uint)(span[offset] | (span[offset + 1] << 8) | (span[offset + 2] << 16) | (span[offset + 3] << 24));

    private static uint ReadUInt32(ReadOnlySpan<byte> span, ref int offset)
    {
        var v = ReadUInt32(span, offset);
        offset += 4;
        return v;
    }

    private static ulong ReadUInt64(ReadOnlySpan<byte> span, ref int offset)
    {
        ulong lo = ReadUInt32(span, ref offset);
        ulong hi = ReadUInt32(span, ref offset);
        return lo | (hi << 32);
    }

    private static int ListMetadataReaderNestedMembers()
    {
        Console.WriteLine("MetadataReader members matching: Nested|Table|RowCount|Row|GetTable");
        foreach (var m in typeof(MetadataReader)
            .GetMembers(BindingFlags.Public | BindingFlags.Instance)
            .Where(m =>
                m.Name.Contains("Nested", StringComparison.OrdinalIgnoreCase) ||
                m.Name.Contains("Table", StringComparison.OrdinalIgnoreCase) ||
                m.Name.Contains("RowCount", StringComparison.OrdinalIgnoreCase) ||
                m.Name.Contains("GetTable", StringComparison.OrdinalIgnoreCase) ||
                m.Name.Contains("Row", StringComparison.OrdinalIgnoreCase))
            .OrderBy(m => m.MemberType)
            .ThenBy(m => m.Name))
        {
            Console.WriteLine($"  {m.MemberType}: {m.Name}");
        }

        return 0;
    }

    private static int ListMetadataReaderMembersAll()
    {
        Console.WriteLine("MetadataReader public instance members (first 80):");
        var members = typeof(MetadataReader)
            .GetMembers(BindingFlags.Public | BindingFlags.Instance)
            .OrderBy(m => m.MemberType)
            .ThenBy(m => m.Name)
            .Take(80)
            .ToArray();

        foreach (var m in members)
        {
            Console.WriteLine($"  {m.MemberType}: {m.Name}");
        }

        Console.WriteLine($"Total public instance members: {typeof(MetadataReader).GetMembers(BindingFlags.Public | BindingFlags.Instance).Length}");
        return 0;
    }
}
