namespace Mast.Dbo;

public readonly record struct CaseInsensitiveString(string Value)
{
    public bool Equals(CaseInsensitiveString other)
        => string.Equals(Value, other.Value, StringComparison.InvariantCultureIgnoreCase);

    public override int GetHashCode() => Value.GetHashCode();

    public static implicit operator string(CaseInsensitiveString c) => c.Value;

    public static implicit operator CaseInsensitiveString(string s) => new(s);
}
