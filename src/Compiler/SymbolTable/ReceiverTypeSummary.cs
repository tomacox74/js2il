namespace Jroc.SymbolTables;

internal sealed class ReceiverTypeSummary : IEquatable<ReceiverTypeSummary>
{
    public static ReceiverTypeSummary Empty { get; } = new(
        includesUnknown: false,
        includesNonCandidate: false,
        []);

    public static ReceiverTypeSummary Unknown { get; } = new(
        includesUnknown: true,
        includesNonCandidate: true,
        []);

    public static ReceiverTypeSummary NonCandidate { get; } = new(
        includesUnknown: false,
        includesNonCandidate: true,
        []);

    public ReceiverTypeSummary(
        bool includesUnknown,
        bool includesNonCandidate,
        IEnumerable<Type> candidateClrTypes)
    {
        IncludesUnknown = includesUnknown;
        IncludesNonCandidate = includesNonCandidate;
        CandidateClrTypes = new HashSet<Type>(candidateClrTypes);
    }

    public bool IncludesUnknown { get; }
    public bool IncludesNonCandidate { get; }
    public IReadOnlySet<Type> CandidateClrTypes { get; }
    public bool HasCandidates => CandidateClrTypes.Count > 0;
    public bool IsEmpty => !IncludesUnknown
        && !IncludesNonCandidate
        && CandidateClrTypes.Count == 0;

    public static ReceiverTypeSummary ForCandidate(Type type)
        => new(
            includesUnknown: false,
            includesNonCandidate: false,
            [type]);

    public ReceiverTypeSummary Union(ReceiverTypeSummary other)
        => new(
            IncludesUnknown || other.IncludesUnknown,
            IncludesNonCandidate || other.IncludesNonCandidate,
            CandidateClrTypes.Union(other.CandidateClrTypes));

    public bool Equals(ReceiverTypeSummary? other)
        => other != null
            && IncludesUnknown == other.IncludesUnknown
            && IncludesNonCandidate == other.IncludesNonCandidate
            && CandidateClrTypes.SetEquals(other.CandidateClrTypes);

    public override bool Equals(object? obj)
        => obj is ReceiverTypeSummary other && Equals(other);

    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(IncludesUnknown);
        hash.Add(IncludesNonCandidate);
        foreach (var type in CandidateClrTypes.OrderBy(
                     static type => type.FullName,
                     StringComparer.Ordinal))
        {
            hash.Add(type);
        }

        return hash.ToHashCode();
    }
}
