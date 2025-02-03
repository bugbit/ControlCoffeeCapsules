using System;

[Serializable]
public struct JsonDateTime : IComparable<JsonDateTime>
{
    public long value;

    public int CompareTo(JsonDateTime other) => ((DateTime)this).CompareTo((DateTime)other);

    public static implicit operator DateTime(JsonDateTime jdt) => DateTime.FromFileTimeUtc(jdt.value);
    public static implicit operator JsonDateTime(DateTime dt)
    {
        var jdt = new JsonDateTime();

        jdt.value = dt.ToFileTimeUtc();

        return jdt;
    }
}
