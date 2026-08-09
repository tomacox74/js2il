using System.Net;
using System.Net.Sockets;
using JavaScriptRuntime.EngineCore;

namespace JavaScriptRuntime.Node;

[NodeModule("dns")]
public sealed partial class Dns
{
    private static string _defaultResultOrder = "verbatim";

    public object? lookup(object[] args)
    {
        if (args.Length is < 2 or > 3)
        {
            throw new TypeError(
                "The \"callback\" argument must be of type function.");
        }

        if (args[0] is not string hostname)
        {
            throw new TypeError(
                "The \"hostname\" argument must be of type string.");
        }

        var callback = args[^1];
        if (!CallableOperations.IsCallable(callback))
        {
            throw new TypeError(
                "The \"callback\" argument must be of type function.");
        }

        var options = args.Length == 3 ? ParseLookupOptions(args[1]) : new LookupOptions();
        var scheduler = GlobalThis.ServiceProvider?.Resolve<NodeSchedulerState>()
            ?? throw new InvalidOperationException(
                "NodeSchedulerState is not available for dns.lookup.");
        var lifetime = Promise.withResolvers();

        scheduler.BeginIo();
        _ = CompleteLookupAsync(scheduler, lifetime, hostname, options, callback);
        return null;
    }

    public string getDefaultResultOrder()
        => GetDefaultResultOrder();

    public void setDefaultResultOrder(string order)
    {
        ValidateResultOrder(order, "order");
        Volatile.Write(ref _defaultResultOrder, order);
    }

    private static async Task CompleteLookupAsync(
        NodeSchedulerState scheduler,
        PromiseWithResolvers lifetime,
        string hostname,
        LookupOptions options,
        object callback)
    {
        try
        {
            var addresses = await Task.Run(
                () => ResolveAddresses(hostname, options)).ConfigureAwait(false);
            ((IScheduler)scheduler).ScheduleImmediate(() =>
            {
                try
                {
                    if (options.All)
                    {
                        CallableOperations.Call2(
                            callback,
                            null,
                            JsNull.Null,
                            CreateAddressRecords(addresses));
                    }
                    else
                    {
                        var address = addresses[0];
                        CallableOperations.Call3(
                            callback,
                            null,
                            JsNull.Null,
                            address.ToString(),
                            GetFamily(address));
                    }
                }
                finally
                {
                    scheduler.EndIo(lifetime, null);
                }
            });
        }
        catch (Exception ex)
        {
            var error = new DnsLookupError(hostname, ex);
            ((IScheduler)scheduler).ScheduleImmediate(() =>
            {
                try
                {
                    CallableOperations.Call1(callback, null, error);
                }
                finally
                {
                    scheduler.EndIo(lifetime, null);
                }
            });
        }
    }

    private static IPAddress[] ResolveAddresses(string hostname, LookupOptions options)
    {
        var addresses = IPAddress.TryParse(hostname, out var literal)
            ? [literal]
            : System.Net.Dns.GetHostAddresses(hostname);
        var filtered = addresses
            .Where(address => options.Family == 0 || GetFamily(address) == options.Family)
            .Distinct()
            .ToArray();

        if (filtered.Length == 0)
        {
            throw new SocketException((int)SocketError.HostNotFound);
        }

        return OrderAddresses(filtered, options.Order);
    }

    private static IPAddress[] OrderAddresses(
        IEnumerable<IPAddress> addresses,
        string order)
        => order switch
        {
            "verbatim" => addresses.ToArray(),
            "ipv4first" => addresses.OrderBy(address => GetFamily(address) == 4 ? 0 : 1).ToArray(),
            "ipv6first" => addresses.OrderBy(address => GetFamily(address) == 6 ? 0 : 1).ToArray(),
            _ => throw new InvalidOperationException($"Unsupported DNS result order '{order}'.")
        };

    private static JavaScriptRuntime.Array CreateAddressRecords(IEnumerable<IPAddress> addresses)
    {
        var records = new JavaScriptRuntime.Array();
        foreach (var address in addresses)
        {
            var record = new JsObject();
            ObjectRuntime.SetProperty(record, "address", address.ToString());
            ObjectRuntime.SetProperty(record, "family", (double)GetFamily(address));
            records.Add(record);
        }

        return records;
    }

    private static LookupOptions ParseLookupOptions(object? options)
    {
        if (options is null or JsNull)
        {
            return new LookupOptions();
        }

        if (options is double or int or long or short)
        {
            return new LookupOptions
            {
                Family = ParseFamily(options, "family")
            };
        }

        if (options is string || !NodeNetworkingCommon.LooksLikeOptionsObject(options))
        {
            throw new TypeError(
                "The \"options\" argument must be of type integer or object.");
        }

        var family = ReadOption(options, "family");
        var all = ReadOption(options, "all");
        var verbatim = ReadOption(options, "verbatim");
        var order = ReadOption(options, "order");
        var hints = ReadOption(options, "hints");

        if (hints is not null and not JsNull
            && TypeUtilities.ToNumber(hints) != 0)
        {
            throw new NotSupportedException(
                "node:dns lookup hints are not implemented.");
        }

        var resultOrder = verbatim is not null and not JsNull
            ? ReadBooleanOption(verbatim, "options.verbatim") ? "verbatim" : "ipv4first"
            : GetDefaultResultOrder();
        if (order is not null and not JsNull)
        {
            if (order is not string orderText)
            {
                throw new TypeError(
                    "The \"options.order\" property must be of type string.");
            }

            resultOrder = orderText;
        }

        ValidateResultOrder(resultOrder, "options.order");
        return new LookupOptions
        {
            Family = family is null or JsNull ? 0 : ParseFamily(family, "options.family"),
            All = all is not null and not JsNull && ReadBooleanOption(all, "options.all"),
            Order = resultOrder
        };
    }

    private static object? ReadOption(object options, string name)
        => ObjectRuntime.GetProperty(options, name);

    private static int ParseFamily(object family, string argumentName)
    {
        if (family is string text)
        {
            return text switch
            {
                "IPv4" => 4,
                "IPv6" => 6,
                _ => throw new TypeError(
                    $"The \"{argumentName}\" argument must be 0, 4, 6, \"IPv4\", or \"IPv6\".")
            };
        }

        var number = TypeUtilities.ToNumber(family);
        if (number is 0 or 4 or 6)
        {
            return (int)number;
        }

        throw new TypeError(
            $"The \"{argumentName}\" argument must be 0, 4, 6, \"IPv4\", or \"IPv6\".");
    }

    private static bool ReadBooleanOption(object value, string argumentName)
    {
        if (value is bool boolean)
        {
            return boolean;
        }

        throw new TypeError(
            $"The \"{argumentName}\" property must be of type boolean.");
    }

    private static void ValidateResultOrder(string order, string argumentName)
    {
        if (order is not ("verbatim" or "ipv4first" or "ipv6first"))
        {
            throw new TypeError(
                $"The \"{argumentName}\" argument must be \"verbatim\", \"ipv4first\", or \"ipv6first\".");
        }
    }

    private static int GetFamily(IPAddress address)
        => address.AddressFamily == AddressFamily.InterNetwork ? 4 : 6;

    private static string GetDefaultResultOrder()
        => Volatile.Read(ref _defaultResultOrder);

    private sealed class LookupOptions
    {
        public bool All { get; init; }

        public int Family { get; init; }

        public string Order { get; init; } = GetDefaultResultOrder();
    }

    private sealed class DnsLookupError(string hostname, Exception innerException)
        : Error($"getaddrinfo ENOTFOUND {hostname}", innerException)
    {
        public string code => "ENOTFOUND";

        public string hostname => hostname;

        public string syscall => "getaddrinfo";
    }
}
