using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;
using System.Runtime.Loader;

namespace MemberReferencePoC;

internal static class Program
{
    private static int Main(string[] args)
    {
        var outputPath = args.Length > 0
            ? Path.GetFullPath(args[0])
            : Path.Combine(AppContext.BaseDirectory, "Generated.MemberRefPoC.dll");

        var coreLibName = typeof(object).Assembly.GetName().Name
            ?? throw new InvalidOperationException("Could not determine core library assembly name");

        Console.WriteLine($"Core library assembly name: {coreLibName}");

        var result = SimpleAssemblyEmitter.EmitLibrary(outputPath, coreLibName);

        Console.WriteLine($"Wrote: {outputPath}");
        Console.WriteLine($"AddOne MethodDef token: 0x{result.AddOneMethodDefToken:X8} (0x06xxxxxx = MethodDef)");
        Console.WriteLine($"AddOne MemberRef token: 0x{result.AddOneMemberRefToken:X8} (0x0Axxxxxx = MemberRef)");

        // Load and execute
        var alc = new AssemblyLoadContext("GeneratedMemberRefPoC", isCollectible: true);
        try
        {
            var asm = alc.LoadFromAssemblyPath(outputPath);
            var demoType = asm.GetType("Generated.Demo", throwOnError: true)!;
            var callViaMethodDef = demoType.GetMethod("CallViaMethodDef", BindingFlags.Public | BindingFlags.Static)!;
            var callViaMemberRef = demoType.GetMethod("CallViaMemberRef", BindingFlags.Public | BindingFlags.Static)!;

            var value1 = (int)callViaMethodDef.Invoke(null, new object[] { 41 })!;
            Console.WriteLine($"Generated.Demo.CallViaMethodDef(41) => {value1}");

            var value2 = (int)callViaMemberRef.Invoke(null, new object[] { 41 })!;
            Console.WriteLine($"Generated.Demo.CallViaMemberRef(41) => {value2}");

            if (value1 != 42 || value2 != 42)
            {
                Console.Error.WriteLine("FAIL: expected 42");
                return 1;
            }

            Console.WriteLine("OK");
            return 0;
        }
        finally
        {
            alc.Unload();
        }
    }
}

internal sealed record EmitResult(int AddOneMethodDefToken, int AddOneMemberRefToken);

internal static class SimpleAssemblyEmitter
{
    public static EmitResult EmitLibrary(string outputPath, string coreLibAssemblyName)
    {
        var metadata = new MetadataBuilder();
        var il = new BlobBuilder();
        var methodBodyStream = new BlobBuilder();

        // Basic assembly/module
        var assemblyName = metadata.GetOrAddString("Generated.MemberRefPoC");
        var moduleName = metadata.GetOrAddString(Path.GetFileName(outputPath));

        var mvid = Guid.NewGuid();

        metadata.AddModule(
            0,
            moduleName,
            metadata.GetOrAddGuid(mvid),
            default,
            default);

        var coreLibRef = AddAssemblyReference(metadata, coreLibAssemblyName);

        // System.Object type ref
        var systemNs = metadata.GetOrAddString("System");
        var objectName = metadata.GetOrAddString("Object");
        var objectTypeRef = metadata.AddTypeReference(coreLibRef, systemNs, objectName);

        // System.Int32 type ref
        var int32Name = metadata.GetOrAddString("Int32");
        var int32TypeRef = metadata.AddTypeReference(coreLibRef, systemNs, int32Name);

        // Create the assembly AFTER module (ok order-wise; handles are just tokens)
        metadata.AddAssembly(
            name: assemblyName,
            version: new Version(1, 0, 0, 0),
            culture: default,
            publicKey: default,
            flags: 0,
            hashAlgorithm: AssemblyHashAlgorithm.None);

        // <Module>
        var moduleTypeDef = metadata.AddTypeDefinition(
            attributes: TypeAttributes.NotPublic,
            @namespace: default,
            name: metadata.GetOrAddString("<Module>"),
            baseType: default,
            fieldList: MetadataTokens.FieldDefinitionHandle(1),
            methodList: MetadataTokens.MethodDefinitionHandle(1));

        // Generated.Demo : System.Object
        var generatedNs = metadata.GetOrAddString("Generated");
        var demoName = metadata.GetOrAddString("Demo");

        // We will add methods next; methodList points at first method row.
        var demoTypeDef = metadata.AddTypeDefinition(
            attributes: TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.Abstract, // static class
            @namespace: generatedNs,
            name: demoName,
            baseType: objectTypeRef,
            fieldList: MetadataTokens.FieldDefinitionHandle(1),
            methodList: MetadataTokens.MethodDefinitionHandle(1));

        // Signatures
        var addOneSig = BuildStaticMethodSignatureInt32ToInt32(metadata, int32TypeRef);

        // We'll intentionally embed the MethodDef token for row 3 into IL BEFORE the MethodDef row exists.
        // This mirrors what a compiler can do internally (allocate rows up-front, then fill bodies later).
        // IMPORTANT: This only works because we will add the MethodDefinition rows in a deterministic order.
        // We are going to add AddOne LAST, so its MethodDef row will be #3.
        var assumedAddOneMethodDef = MetadataTokens.MethodDefinitionHandle(3);
        var assumedAddOneMethodDefToken = MetadataTokens.GetToken(assumedAddOneMethodDef);

        // IMPORTANT: Create a MemberReference to AddOne BEFORE AddOne's MethodDefinition exists.
        var addOneMemberRef = metadata.AddMemberReference(
            parent: demoTypeDef,
            name: metadata.GetOrAddString("AddOne"),
            signature: addOneSig);

        // Build bodies first (so we have stable offsets), then add MethodDef rows.
        // Body order is independent of MethodDef row order.
        var callViaMethodDefBodyOffset = CallViaMethodDefBody(methodBodyStream, assumedAddOneMethodDefToken);
        var callViaMemberRefBodyOffset = CallViaMemberRefBody(methodBodyStream, addOneMemberRef);
        var addOneBodyOffset = AddOneBody(methodBodyStream);

        // MethodDef row 1: CallViaMethodDef
        var callViaMethodDefSig = BuildStaticMethodSignatureInt32ToInt32(metadata, int32TypeRef);
        var callViaMethodDefMethodDef = metadata.AddMethodDefinition(
            attributes: MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig,
            implAttributes: MethodImplAttributes.IL | MethodImplAttributes.Managed,
            name: metadata.GetOrAddString("CallViaMethodDef"),
            signature: callViaMethodDefSig,
            bodyOffset: callViaMethodDefBodyOffset,
            parameterList: MetadataTokens.ParameterHandle(1));

        // MethodDef row 2: CallViaMemberRef
        var callViaMemberRefSig = BuildStaticMethodSignatureInt32ToInt32(metadata, int32TypeRef);
        var callViaMemberRefMethodDef = metadata.AddMethodDefinition(
            attributes: MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig,
            implAttributes: MethodImplAttributes.IL | MethodImplAttributes.Managed,
            name: metadata.GetOrAddString("CallViaMemberRef"),
            signature: callViaMemberRefSig,
            bodyOffset: callViaMemberRefBodyOffset,
            parameterList: MetadataTokens.ParameterHandle(3));

        // MethodDef row 3: AddOne (intentionally last)
        var addOneMethodDef = metadata.AddMethodDefinition(
            attributes: MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig,
            implAttributes: MethodImplAttributes.IL | MethodImplAttributes.Managed,
            name: metadata.GetOrAddString("AddOne"),
            signature: addOneSig,
            bodyOffset: addOneBodyOffset,
            parameterList: MetadataTokens.ParameterHandle(5));

        // Parameter rows.
        // Parameter table must be ordered by method definition order. Use an explicit return ParamDef (sequence 0)
        // for each method to keep tooling happy.
        metadata.AddParameter(ParameterAttributes.None, default, sequenceNumber: 0); // CallViaMethodDef return
        metadata.AddParameter(ParameterAttributes.None, metadata.GetOrAddString("x"), sequenceNumber: 1);
        metadata.AddParameter(ParameterAttributes.None, default, sequenceNumber: 0); // CallViaMemberRef return
        metadata.AddParameter(ParameterAttributes.None, metadata.GetOrAddString("x"), sequenceNumber: 1);
        metadata.AddParameter(ParameterAttributes.None, default, sequenceNumber: 0); // AddOne return
        metadata.AddParameter(ParameterAttributes.None, metadata.GetOrAddString("x"), sequenceNumber: 1);

        // Mark DemoType's method list range is implicit by ordering; add the type defs in the right sequence already.

        // Method bodies + metadata -> PE
        var peImage = BuildPeImage(metadata, methodBodyStream);
        Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
        File.WriteAllBytes(outputPath, peImage);

        var actualAddOneMethodDefToken = MetadataTokens.GetToken(addOneMethodDef);
        if (actualAddOneMethodDefToken != assumedAddOneMethodDefToken)
        {
            throw new InvalidOperationException(
                $"Unexpected MethodDef token for AddOne. Assumed 0x{assumedAddOneMethodDefToken:X8}, got 0x{actualAddOneMethodDefToken:X8}.");
        }

        return new EmitResult(
            AddOneMethodDefToken: actualAddOneMethodDefToken,
            AddOneMemberRefToken: MetadataTokens.GetToken(addOneMemberRef));
    }

    private static AssemblyReferenceHandle AddAssemblyReference(MetadataBuilder metadata, string name)
    {
        return metadata.AddAssemblyReference(
            name: metadata.GetOrAddString(name),
            version: new Version(0, 0, 0, 0),
            culture: default,
            publicKeyOrToken: default,
            flags: 0,
            hashValue: default);
    }

    private static BlobHandle BuildStaticMethodSignatureInt32ToInt32(MetadataBuilder metadata, TypeReferenceHandle int32TypeRef)
    {
        // static int32 (int32)
        var sig = new BlobBuilder();
        var encoder = new BlobEncoder(sig);
        var methodSig = encoder.MethodSignature(isInstanceMethod: false);
        methodSig.Parameters(
            parameterCount: 1,
            returnType => returnType.Type().Int32(),
            parameters =>
            {
                parameters.AddParameter().Type().Int32();
            });
        return metadata.GetOrAddBlob(sig);
    }

    private static int AddOneBody(BlobBuilder methodBodyStream)
    {
        // IL: ldarg.0; ldc.i4.1; add; ret
        var il = new BlobBuilder();
        il.OpCode(ILOpCode.Ldarg_0);
        il.OpCode(ILOpCode.Ldc_I4_1);
        il.OpCode(ILOpCode.Add);
        il.OpCode(ILOpCode.Ret);

        return AddMethodBody(methodBodyStream, il, maxStack: 2);
    }

    private static int CallDirectBody(BlobBuilder methodBodyStream, MemberReferenceHandle addOneMemberRef)
    {
        // Legacy helper kept for reference; prefer CallViaMemberRefBody.
        return CallViaMemberRefBody(methodBodyStream, addOneMemberRef);
    }

    private static int CallViaMethodDefBody(BlobBuilder methodBodyStream, int addOneMethodDefToken)
    {
        // IL: ldarg.0; call methoddef(AddOne); ret
        var il = new BlobBuilder();
        il.OpCode(ILOpCode.Ldarg_0);
        il.OpCode(ILOpCode.Call);
        il.Token(addOneMethodDefToken);
        il.OpCode(ILOpCode.Ret);

        return AddMethodBody(methodBodyStream, il, maxStack: 1);
    }

    private static int CallViaMemberRefBody(BlobBuilder methodBodyStream, MemberReferenceHandle addOneMemberRef)
    {
        // IL: ldarg.0; call memberref(AddOne); ret
        var il = new BlobBuilder();
        il.OpCode(ILOpCode.Ldarg_0);
        il.OpCode(ILOpCode.Call);
        il.Token(MetadataTokens.GetToken(addOneMemberRef));
        il.OpCode(ILOpCode.Ret);

        return AddMethodBody(methodBodyStream, il, maxStack: 1);
    }

    private static int AddMethodBody(BlobBuilder methodBodyStream, BlobBuilder il, int maxStack)
    {
        // Very small, fat header always (simpler POC)
        // Fat header: 12 bytes
        //  2 bytes: flags+size (0x3003 => fat, 3 dwords)
        //  2 bytes: maxStack
        //  4 bytes: codeSize
        //  4 bytes: localVarSigTok
        var body = new BlobBuilder();
        const ushort flagsAndSize = 0x3003;
        body.WriteUInt16(flagsAndSize);
        body.WriteUInt16((ushort)maxStack);
        body.WriteInt32(il.Count);
        body.WriteInt32(0); // no locals
        body.WriteBytes(il.ToArray());

        var offset = methodBodyStream.Count;
        methodBodyStream.WriteBytes(body.ToArray());

        // Align to 4 bytes
        while ((methodBodyStream.Count & 3) != 0)
        {
            methodBodyStream.WriteByte(0);
        }

        return offset;
    }

    private static byte[] BuildPeImage(MetadataBuilder metadata, BlobBuilder ilStream)
    {
        var metadataRootBuilder = new MetadataRootBuilder(metadata);

        var peBuilder = new ManagedPEBuilder(
            header: PEHeaderBuilder.CreateLibraryHeader(),
            metadataRootBuilder: metadataRootBuilder,
            ilStream: ilStream,
            mappedFieldData: null,
            entryPoint: default,
            flags: CorFlags.ILOnly);

        var peBlob = new BlobBuilder();
        peBuilder.Serialize(peBlob);
        return peBlob.ToArray();
    }
}

internal enum ILOpCode : byte
{
    // Single-byte opcode subset needed for this PoC.
    Ldarg_0 = 0x02,
    Ldc_I4_1 = 0x17,
    Call = 0x28,
    Ret = 0x2A,
    Add = 0x58,
}

internal static class IlBuilderExtensions
{
    public static BlobBuilder OpCode(this BlobBuilder il, ILOpCode opCode)
    {
        il.WriteByte((byte)opCode);
        return il;
    }

    public static BlobBuilder Token(this BlobBuilder il, int metadataToken)
    {
        il.WriteInt32(metadataToken);
        return il;
    }
}
