using System.Reflection;

namespace Jroc.Runtime;

internal static class GeneratedContractMetadata
{
    private const string GeneratedAttributeNamespace = "Jroc.Generated.Metadata";

    internal static string? GetModuleId(Type contractType)
    {
        var runtimeAttribute = contractType.GetCustomAttribute<JsModuleAttribute>();
        if (runtimeAttribute != null)
        {
            return runtimeAttribute.ModuleId;
        }

        return GetStringConstructorArgument(contractType, "JsModuleAttribute");
    }

    internal static string? GetExportName(MethodInfo method)
    {
        var runtimeAttribute = method.GetCustomAttribute<JsExportNameAttribute>();
        if (runtimeAttribute != null)
        {
            return runtimeAttribute.ExportName;
        }

        return GetStringConstructorArgument(method, "JsExportNameAttribute");
    }

    internal static bool IsExportValue(MethodInfo method)
        => method.GetCustomAttribute<JsExportValueAttribute>() != null
           || HasGeneratedAttribute(method, "JsExportValueAttribute");

    internal static bool IsGeneratedContractType(Type? contractType)
        => contractType?.Assembly.GetCustomAttributesData().Any(attribute =>
            string.Equals(
                attribute.AttributeType.FullName,
                typeof(JsCompiledModuleAttribute).FullName,
                StringComparison.Ordinal)) == true;

    internal static bool IsObjectContract(Type? contractType)
        => HasGeneratedAttribute(contractType, "JsObjectContractAttribute");

    internal static bool IsArrayContract(Type? contractType)
        => HasGeneratedAttribute(contractType, "JsArrayContractAttribute");

    internal static bool IsCallableContract(Type? contractType)
        => HasGeneratedAttribute(contractType, "JsCallableContractAttribute");

    internal static string? GetBuiltinContractKind(Type? contractType)
        => contractType == null
            ? null
            : GetStringConstructorArgument(contractType, "JsBuiltinContractAttribute");

    internal static Type? GetSiblingObjectContract(Type? contractType)
        => GetSiblingGeneratedHandleContract(contractType, "IObject");

    internal static Type? GetSiblingCallableContract(Type? contractType)
        => GetSiblingGeneratedHandleContract(contractType, "ICallable");

    private static Type? GetSiblingGeneratedHandleContract(
        Type? contractType,
        string name)
    {
        var siblingContract = contractType?.DeclaringType?.GetNestedType(
            name,
            BindingFlags.Public | BindingFlags.NonPublic);
        return IsGeneratedContractType(siblingContract)
               && siblingContract!.IsInterface
               && typeof(IDisposable).IsAssignableFrom(siblingContract)
            ? siblingContract
            : null;
    }

    private static string? GetStringConstructorArgument(MemberInfo member, string attributeName)
        => member
            .GetCustomAttributesData()
            .FirstOrDefault(attribute => IsGeneratedAttribute(attribute, attributeName))
            ?.ConstructorArguments
            .FirstOrDefault()
            .Value as string;

    private static bool HasGeneratedAttribute(MemberInfo? member, string attributeName)
        => member?
            .GetCustomAttributesData()
            .Any(attribute => IsGeneratedAttribute(attribute, attributeName)) == true;

    private static bool IsGeneratedAttribute(CustomAttributeData attribute, string attributeName)
        => string.Equals(
            attribute.AttributeType.FullName,
            $"{GeneratedAttributeNamespace}.{attributeName}",
            StringComparison.Ordinal);
}
