using System;

[Serializable]
public struct JsonDateTime
{
    public long value;
    public static implicit operator DateTime(JsonDateTime jdt) => DateTime.FromFileTimeUtc(jdt.value);
    public static implicit operator JsonDateTime(DateTime dt)
    {
        var jdt = new JsonDateTime();

        jdt.value = dt.ToFileTimeUtc();

        return jdt;
    }
}
